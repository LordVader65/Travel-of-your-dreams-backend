import { NativeStackScreenProps } from '@react-navigation/native-stack';
import { useState } from 'react';
import { Alert, StyleSheet, Text, View } from 'react-native';

import { useAuth } from '../auth/AuthContext';
import { Button, Card, Field, Screen, StatusBadge } from '../components/ui';
import { RootStackParamList } from '../navigation/types';
import { colors } from '../theme/colors';

type Props = NativeStackScreenProps<RootStackParamList, 'Payment'>;

export function PaymentScreen({ route, navigation }: Props) {
  const { reservation } = route.params;
  const { api } = useAuth();
  const [holder, setHolder] = useState('');
  const [card, setCard] = useState('');
  const [expiry, setExpiry] = useState('');
  const [cvv, setCvv] = useState('');
  const [loading, setLoading] = useState(false);
  const [state, setState] = useState('');

  async function pay() {
    if (holder.trim().length < 3 || card.replace(/\s/g, '').length < 16 || !/^\d{2}\/\d{2}$/.test(expiry) || !/^\d{3,4}$/.test(cvv)) {
      Alert.alert('Datos incompletos', 'Revisa titular, número de tarjeta, expiración y CVV.');
      return;
    }
    setLoading(true);
    setState('Enviando pago...');
    try {
      const accepted = await api.requestPayment({ reservationGuid: reservation.guid, amount: reservation.total });
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

  return (
    <Screen>
      <View><Text style={styles.eyebrow}>PAGO SEGURO</Text><Text style={styles.title}>{reservation.code}</Text><Text style={styles.muted}>{reservation.attractionName}</Text></View>
      <Card>
        <Text style={styles.sectionTitle}>Resumen</Text>
        <View style={styles.amount}><Text style={styles.muted}>Total a pagar</Text><Text style={styles.total}>{reservation.total.toFixed(2)} {reservation.currency}</Text></View>
        <StatusBadge status={reservation.status} />
      </Card>
      <Card>
        <View style={styles.cardHeading}><Text style={styles.sectionTitle}>Tarjeta</Text><Text style={styles.brand}>{detectBrand(card)}</Text></View>
        <Field autoCapitalize="words" label="Titular" onChangeText={setHolder} value={holder} />
        <Field keyboardType="number-pad" label="Número de tarjeta" maxLength={19} onChangeText={(value) => setCard(formatCard(value))} placeholder="4111 1111 1111 1111" value={card} />
        <View style={styles.twoColumns}>
          <View style={styles.flex}><Field keyboardType="number-pad" label="Expira" maxLength={5} onChangeText={(value) => setExpiry(formatExpiry(value))} placeholder="MM/AA" value={expiry} /></View>
          <View style={styles.flex}><Field keyboardType="number-pad" label="CVV" maxLength={4} onChangeText={(value) => setCvv(value.replace(/\D/g, ''))} secureTextEntry value={cvv} /></View>
        </View>
        {state ? <View style={styles.process}><StatusBadge status={loading ? 'PROCESANDO' : state} /><Text style={styles.muted}>{state}</Text></View> : null}
        <Button icon="lock-closed-outline" label="Confirmar pago" loading={loading} onPress={pay} />
      </Card>
    </Screen>
  );
}

function formatCard(value: string) { return value.replace(/\D/g, '').slice(0, 16).replace(/(.{4})/g, '$1 ').trim(); }
function formatExpiry(value: string) { const digits = value.replace(/\D/g, '').slice(0, 4); return digits.length > 2 ? `${digits.slice(0, 2)}/${digits.slice(2)}` : digits; }
function detectBrand(value: string) { const digits = value.replace(/\D/g, ''); return digits.startsWith('4') ? 'VISA' : digits.startsWith('5') ? 'MASTERCARD' : 'TARJETA'; }

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
});
