import { NativeStackScreenProps } from '@react-navigation/native-stack';
import { useEffect, useState } from 'react';
import { Alert, StyleSheet, Text, View } from 'react-native';

import { useAuth } from '../auth/AuthContext';
import { Button, Card, Chip, Field, Screen, StatusBadge } from '../components/ui';
import { RootStackParamList } from '../navigation/types';
import { colors } from '../theme/colors';
import type { BillingData, BillingDataInput, IdentificationType } from '../types/models';

type Props = NativeStackScreenProps<RootStackParamList, 'Payment'>;

const identificationTypes: Array<{ value: IdentificationType; label: string }> = [
  { value: 'CEDULA', label: 'Cedula' },
  { value: 'RUC', label: 'RUC' },
  { value: 'PASAPORTE', label: 'Pasaporte' },
];

export function PaymentScreen({ route, navigation }: Props) {
  const { reservation } = route.params;
  const { api } = useAuth();
  const [billingItems, setBillingItems] = useState<BillingData[]>([]);
  const [selectedBillingGuid, setSelectedBillingGuid] = useState('');
  const [billing, setBilling] = useState<BillingDataInput>({
    identificationType: 'CEDULA',
    identificationNumber: '',
    businessName: '',
    name: '',
    lastName: '',
    email: '',
    phone: '',
    address: '',
  });
  const [holder, setHolder] = useState('');
  const [card, setCard] = useState('');
  const [expiry, setExpiry] = useState('');
  const [cvv, setCvv] = useState('');
  const [loading, setLoading] = useState(false);
  const [state, setState] = useState('');

  useEffect(() => {
    let mounted = true;
    Promise.allSettled([api.billingData(), api.profile()])
      .then(([billingResult, profileResult]) => {
        if (!mounted) return;

        if (billingResult.status === 'fulfilled') {
          setBillingItems(billingResult.value);
          const first = billingResult.value[0];
          if (first) {
            setSelectedBillingGuid(first.guid);
            setBilling(billingFromExisting(first));
            return;
          }
        }

        if (profileResult.status === 'fulfilled') {
          const profile = profileResult.value;
          setBilling((current) => ({
            ...current,
            identificationType: normalizeIdentificationType(profile.identificationType),
            identificationNumber: onlyDigits(profile.identificationNumber ?? '', 30),
            businessName: profile.businessName ?? '',
            name: profile.names ?? profile.displayName ?? '',
            lastName: profile.lastNames ?? '',
            email: profile.email ?? '',
            phone: onlyDigits(profile.phone ?? '', 10),
            address: profile.address ?? '',
          }));
        }
      })
      .catch(() => undefined);

    return () => {
      mounted = false;
    };
  }, [api]);

  async function pay() {
    const billingValidation = validateBilling();
    if (billingValidation) {
      Alert.alert('Datos de facturacion incompletos', billingValidation);
      return;
    }
    if (holder.trim().length < 3 || card.replace(/\s/g, '').length < 16 || !/^\d{2}\/\d{2}$/.test(expiry) || !/^\d{3,4}$/.test(cvv)) {
      Alert.alert('Datos incompletos', 'Revisa titular, numero de tarjeta, expiracion y CVV.');
      return;
    }

    setLoading(true);
    setState('Enviando pago...');
    try {
      const billingGuid = selectedBillingGuid || (await api.createBillingData(billing)).guid;
      setSelectedBillingGuid(billingGuid);
      const accepted = await api.requestPayment({
        reservationGuid: reservation.guid,
        amount: reservation.total,
        billingDataGuid: billingGuid,
      });
      setState('Confirmando pago y emitiendo factura...');
      const result = await api.waitForPayment(accepted.correlationId);
      if (result.state !== 'FACTURA_EMITIDA') throw new Error(result.error || 'El pago fue rechazado.');
      setState('FACTURA EMITIDA');
      Alert.alert('Pago aprobado', `Factura ${result.invoiceNumber ?? ''} emitida correctamente.`, [
        { text: 'Ver actividad', onPress: () => navigation.popToTop() },
      ]);
    } catch (error) {
      setState('');
      Alert.alert('No se pudo procesar el pago', error instanceof Error ? error.message : 'Intenta nuevamente.');
    } finally {
      setLoading(false);
    }
  }

  function selectBilling(guid: string) {
    setSelectedBillingGuid(guid);
    const item = billingItems.find((value) => value.guid === guid);
    if (item) setBilling(billingFromExisting(item));
  }

  function useNewBilling() {
    setSelectedBillingGuid('');
    setBilling((current) => ({
      ...current,
      identificationNumber: '',
      businessName: '',
      name: '',
      lastName: '',
      phone: '',
      address: '',
    }));
  }

  function updateBilling(patch: Partial<BillingDataInput>) {
    if (selectedBillingGuid) setSelectedBillingGuid('');
    setBilling((current) => ({ ...current, ...patch }));
  }

  function validateBilling() {
    if (!billing.identificationNumber.trim()) return 'Ingresa el numero de identificacion.';
    if (!billing.name.trim()) return 'Ingresa el nombre del cliente.';
    if (!billing.email.trim() || !billing.email.includes('@')) return 'Ingresa un correo valido para la factura.';
    return '';
  }

  return (
    <Screen>
      <View>
        <Text style={styles.eyebrow}>PAGO SEGURO</Text>
        <Text style={styles.title}>{reservation.code}</Text>
        <Text style={styles.muted}>{reservation.attractionName}</Text>
      </View>

      <Card>
        <Text style={styles.sectionTitle}>Resumen</Text>
        <View style={styles.amount}>
          <Text style={styles.muted}>Total a pagar</Text>
          <Text style={styles.total}>{reservation.total.toFixed(2)} {reservation.currency}</Text>
        </View>
        <StatusBadge status={reservation.status} />
      </Card>

      <Card>
        <Text style={styles.sectionTitle}>Datos de facturacion</Text>
        {billingItems.length ? (
          <View style={styles.chipRow}>
            {billingItems.map((item) => (
              <Chip
                active={selectedBillingGuid === item.guid}
                key={item.guid}
                label={billingLabel(item)}
                onPress={() => selectBilling(item.guid)}
              />
            ))}
            <Chip active={!selectedBillingGuid} label="Nuevos datos" onPress={useNewBilling} />
          </View>
        ) : null}

        <Text style={styles.label}>Tipo identificacion</Text>
        <View style={styles.chipRow}>
          {identificationTypes.map((item) => (
            <Chip
              active={billing.identificationType === item.value}
              key={item.value}
              label={item.label}
              onPress={() => updateBilling({ identificationType: item.value })}
            />
          ))}
        </View>

        <Field
          keyboardType="number-pad"
          label="Numero identificacion"
          maxLength={30}
          onChangeText={(value) => updateBilling({ identificationNumber: onlyDigits(value, 30) })}
          value={billing.identificationNumber}
        />
        <Field label="Nombre" maxLength={100} onChangeText={(value) => updateBilling({ name: value.slice(0, 100) })} value={billing.name} />
        <Field label="Apellido" maxLength={100} onChangeText={(value) => updateBilling({ lastName: value.slice(0, 100) })} value={billing.lastName} />
        <Field label="Razon social" maxLength={200} onChangeText={(value) => updateBilling({ businessName: value.slice(0, 200) })} value={billing.businessName} />
        <Field autoCapitalize="none" keyboardType="email-address" label="Correo" maxLength={150} onChangeText={(value) => updateBilling({ email: value.slice(0, 150) })} value={billing.email} />
        <Field keyboardType="phone-pad" label="Telefono" maxLength={10} onChangeText={(value) => updateBilling({ phone: onlyDigits(value, 10) })} value={billing.phone} />
        <Field label="Direccion" maxLength={300} onChangeText={(value) => updateBilling({ address: value.slice(0, 300) })} value={billing.address} />
      </Card>

      <Card>
        <View style={styles.cardHeading}>
          <Text style={styles.sectionTitle}>Tarjeta</Text>
          <Text style={styles.brand}>{detectBrand(card)}</Text>
        </View>
        <Field autoCapitalize="words" label="Titular" maxLength={100} onChangeText={setHolder} value={holder} />
        <Field keyboardType="number-pad" label="Numero de tarjeta" maxLength={19} onChangeText={(value) => setCard(formatCard(value))} placeholder="4111 1111 1111 1111" value={card} />
        <View style={styles.twoColumns}>
          <View style={styles.flex}>
            <Field keyboardType="number-pad" label="Expira" maxLength={5} onChangeText={(value) => setExpiry(formatExpiry(value))} placeholder="MM/AA" value={expiry} />
          </View>
          <View style={styles.flex}>
            <Field keyboardType="number-pad" label="CVV" maxLength={4} onChangeText={(value) => setCvv(value.replace(/\D/g, ''))} secureTextEntry value={cvv} />
          </View>
        </View>
        {state ? (
          <View style={styles.process}>
            <StatusBadge status={loading ? 'PROCESANDO' : state} />
            <Text style={styles.muted}>{state}</Text>
          </View>
        ) : null}
        <Button icon="lock-closed-outline" label="Confirmar pago" loading={loading} onPress={pay} />
      </Card>
    </Screen>
  );
}

function formatCard(value: string) {
  return value.replace(/\D/g, '').slice(0, 16).replace(/(.{4})/g, '$1 ').trim();
}

function formatExpiry(value: string) {
  const digits = value.replace(/\D/g, '').slice(0, 4);
  return digits.length > 2 ? `${digits.slice(0, 2)}/${digits.slice(2)}` : digits;
}

function detectBrand(value: string) {
  const digits = value.replace(/\D/g, '');
  if (digits.startsWith('4')) return 'VISA';
  if (digits.startsWith('5')) return 'MASTERCARD';
  return 'TARJETA';
}

function onlyDigits(value: string, maxLength: number) {
  return value.replace(/\D/g, '').slice(0, maxLength);
}

function normalizeIdentificationType(value?: string): IdentificationType {
  return value === 'RUC' || value === 'PASAPORTE' ? value : 'CEDULA';
}

function billingFromExisting(item: BillingData): BillingDataInput {
  return {
    identificationType: normalizeIdentificationType(item.identificationType),
    identificationNumber: item.identificationNumber,
    businessName: item.businessName ?? '',
    name: item.name,
    lastName: item.lastName ?? '',
    email: item.email,
    phone: item.phone ?? '',
    address: item.address ?? '',
  };
}

function billingLabel(item: BillingData) {
  const name = item.businessName || item.name || item.email || 'Datos';
  return `${name} - ${item.identificationNumber}`;
}

const styles = StyleSheet.create({
  flex: { flex: 1 },
  eyebrow: { color: colors.accent, fontSize: 12, fontWeight: '900' },
  title: { color: colors.text, fontSize: 29, fontWeight: '900' },
  muted: { color: colors.muted, fontSize: 13, lineHeight: 19 },
  sectionTitle: { color: colors.text, fontSize: 19, fontWeight: '900' },
  amount: { alignItems: 'center', flexDirection: 'row', justifyContent: 'space-between' },
  total: { color: colors.text, fontSize: 24, fontWeight: '900' },
  cardHeading: { alignItems: 'center', flexDirection: 'row', justifyContent: 'space-between' },
  brand: { color: colors.primary, fontSize: 13, fontWeight: '900' },
  twoColumns: { flexDirection: 'row', gap: 12 },
  process: { alignItems: 'center', flexDirection: 'row', gap: 10 },
  label: { color: colors.muted, fontSize: 13, fontWeight: '800' },
  chipRow: { flexDirection: 'row', flexWrap: 'wrap', gap: 8 },
});
