import { NativeStackScreenProps } from '@react-navigation/native-stack';
import { useMemo, useState } from 'react';
import { Alert, Pressable, StyleSheet, Text, View } from 'react-native';

import { useAuth } from '../auth/AuthContext';
import { Button, Card, Chip, Screen, StatusBadge } from '../components/ui';
import { RootStackParamList } from '../navigation/types';
import { colors } from '../theme/colors';

type Props = NativeStackScreenProps<RootStackParamList, 'Checkout'>;
const schedulePageSize = 5;
const fallbackTaxPercentage = 12;

export function CheckoutScreen({ route, navigation }: Props) {
  const { attraction, schedules, tickets } = route.params;
  const { api } = useAuth();
  const [scheduleGuid, setScheduleGuid] = useState(schedules[0]?.guid ?? '');
  const [quantities, setQuantities] = useState<Record<string, number>>(() => Object.fromEntries(tickets.map((x, index) => [x.guid, index === 0 ? 1 : 0])));
  const [schedulePage, setSchedulePage] = useState(0);
  const [processing, setProcessing] = useState(false);
  const [processState, setProcessState] = useState('');

  const selectedSchedule = schedules.find((x) => x.guid === scheduleGuid);
  const schedulePageCount = Math.max(1, Math.ceil(schedules.length / schedulePageSize));
  const visibleSchedules = schedules.slice(schedulePage * schedulePageSize, schedulePage * schedulePageSize + schedulePageSize);
  const taxPercentage = attraction.taxPercentage ?? fallbackTaxPercentage;
  const subtotal = useMemo(() => tickets.reduce((sum, entry) => sum + entry.price * (quantities[entry.guid] ?? 0), 0), [quantities, tickets]);
  const iva = subtotal * taxPercentage / 100;
  const total = subtotal + iva;

  function update(ticketGuid: string, delta: number) {
    setQuantities((current) => ({ ...current, [ticketGuid]: Math.max(0, Math.min(20, (current[ticketGuid] ?? 0) + delta)) }));
  }

  async function reserve() {
    const lines = tickets
      .map((entry) => ({ ticketGuid: entry.guid, quantity: quantities[entry.guid] ?? 0 }))
      .filter((entry) => entry.quantity > 0);
    const quantity = lines.reduce((sum, entry) => sum + entry.quantity, 0);
    if (!scheduleGuid || !lines.length) {
      Alert.alert('Reserva incompleta', 'Selecciona un horario y al menos una entrada.');
      return;
    }
    if (selectedSchedule && quantity > selectedSchedule.availableSlots) {
      Alert.alert('Cupos insuficientes', `El horario permite un máximo de ${selectedSchedule.availableSlots} participantes.`);
      return;
    }

    setProcessing(true);
    setProcessState('Enviando solicitud...');
    try {
      const accepted = await api.requestReservation({ attractionGuid: attraction.guid, scheduleGuid, lines, taxPercentage });
      setProcessState('Confirmando disponibilidad...');
      const result = await api.waitForReservation(accepted.correlationId);
      if (result.state !== 'RESERVA_CREADA' || !result.reservationGuid) {
        throw new Error(result.error || 'La reserva fue rechazada.');
      }
      setProcessState('Reserva creada');
      const created = await api.reservation(result.reservationGuid);
      Alert.alert('Reserva confirmada', `${result.reservationCode ?? created.code} fue creada correctamente.`, [
        { text: 'Ver actividad', onPress: () => navigation.navigate('Main') },
        { text: 'Pagar ahora', onPress: () => navigation.replace('Payment', { reservation: created }) },
      ]);
    } catch (error) {
      Alert.alert('No se pudo reservar', error instanceof Error ? error.message : 'Intenta nuevamente.');
      setProcessState('');
    } finally {
      setProcessing(false);
    }
  }

  return (
    <Screen>
      <View><Text style={styles.eyebrow}>RESERVA</Text><Text style={styles.title}>{attraction.name}</Text></View>
      <Card>
        <Text style={styles.sectionTitle}>1. Elige fecha y horario</Text>
        <View style={styles.optionStack}>
          {visibleSchedules.map((entry) => (
            <Pressable key={entry.guid} onPress={() => setScheduleGuid(entry.guid)} style={[styles.option, entry.guid === scheduleGuid && styles.optionActive]}>
              <View style={styles.flex}><Text style={styles.optionTitle}>{formatDate(entry.date)}</Text><Text style={styles.muted}>{entry.startTime}{entry.endTime ? ` - ${entry.endTime}` : ''}</Text></View>
              <Chip active={entry.guid === scheduleGuid} label={`${entry.availableSlots} cupos`} />
            </Pressable>
          ))}
        </View>
        {schedulePageCount > 1 ? (
          <View style={styles.pagination}>
            <Button
              disabled={schedulePage === 0}
              label="Anterior"
              variant="secondary"
              onPress={() => setSchedulePage((page) => Math.max(0, page - 1))}
            />
            <Text style={styles.pageText}>{schedulePage + 1} / {schedulePageCount}</Text>
            <Button
              disabled={schedulePage >= schedulePageCount - 1}
              label="Siguiente"
              variant="secondary"
              onPress={() => setSchedulePage((page) => Math.min(schedulePageCount - 1, page + 1))}
            />
          </View>
        ) : null}
      </Card>
      <Card>
        <Text style={styles.sectionTitle}>2. Selecciona entradas</Text>
        {tickets.map((entry) => (
          <View key={entry.guid} style={styles.ticketRow}>
            <View style={styles.flex}><Text style={styles.optionTitle}>{entry.title}</Text><Text style={styles.muted}>{entry.price.toFixed(2)} {entry.currency}</Text></View>
            <View style={styles.stepper}>
              <Pressable accessibilityLabel="Reducir cantidad" onPress={() => update(entry.guid, -1)} style={styles.stepButton}><Text style={styles.stepText}>−</Text></Pressable>
              <Text style={styles.quantity}>{quantities[entry.guid] ?? 0}</Text>
              <Pressable accessibilityLabel="Aumentar cantidad" onPress={() => update(entry.guid, 1)} style={styles.stepButton}><Text style={styles.stepText}>+</Text></Pressable>
            </View>
          </View>
        ))}
      </Card>
      <Card>
        <Text style={styles.sectionTitle}>Resumen</Text>
        <Amount label="Subtotal" value={subtotal} />
        <Amount label={`IVA (${formatPercentage(taxPercentage)})`} value={iva} />
        <View style={styles.divider} />
        <Amount label="Total" value={total} strong />
        {processState ? <View style={styles.process}><StatusBadge status={processing ? 'PROCESANDO' : processState} /><Text style={styles.muted}>{processState}</Text></View> : null}
        <Button disabled={!subtotal} icon="checkmark-circle-outline" label="Crear reserva" loading={processing} onPress={reserve} />
      </Card>
    </Screen>
  );
}

function Amount({ label, value, strong }: { label: string; value: number; strong?: boolean }) {
  return <View style={styles.amount}><Text style={[styles.muted, strong && styles.strong]}>{label}</Text><Text style={[styles.amountValue, strong && styles.total]}>{value.toFixed(2)} USD</Text></View>;
}

function formatDate(value: string) {
  return new Intl.DateTimeFormat('es-EC', { weekday: 'short', day: '2-digit', month: 'short' }).format(new Date(`${value}T12:00:00`));
}

function formatPercentage(value: number) {
  return `${Number.isInteger(value) ? value.toFixed(0) : value.toFixed(2)}%`;
}

const styles = StyleSheet.create({
  flex: { flex: 1 },
  eyebrow: { color: colors.accent, fontSize: 12, fontWeight: '900' },
  title: { color: colors.text, fontSize: 29, fontWeight: '900' },
  sectionTitle: { color: colors.text, fontSize: 19, fontWeight: '900' },
  optionStack: { gap: 10 },
  option: { alignItems: 'center', borderColor: colors.line, borderRadius: 7, borderWidth: 1, flexDirection: 'row', gap: 10, padding: 13 },
  optionActive: { backgroundColor: colors.primarySoft, borderColor: colors.primary },
  optionTitle: { color: colors.text, fontSize: 15, fontWeight: '800' },
  pagination: { alignItems: 'center', flexDirection: 'row', gap: 10, justifyContent: 'space-between' },
  pageText: { color: colors.muted, fontSize: 13, fontWeight: '800', minWidth: 44, textAlign: 'center' },
  muted: { color: colors.muted, fontSize: 13 },
  ticketRow: { alignItems: 'center', borderTopColor: colors.line, borderTopWidth: 1, flexDirection: 'row', gap: 12, paddingTop: 12 },
  stepper: { alignItems: 'center', flexDirection: 'row', gap: 12 },
  stepButton: { alignItems: 'center', backgroundColor: colors.surfaceMuted, borderRadius: 7, height: 38, justifyContent: 'center', width: 38 },
  stepText: { color: colors.text, fontSize: 23, fontWeight: '700' },
  quantity: { color: colors.text, fontSize: 16, fontWeight: '900', minWidth: 20, textAlign: 'center' },
  amount: { flexDirection: 'row', justifyContent: 'space-between' },
  amountValue: { color: colors.text, fontSize: 14, fontWeight: '800' },
  strong: { color: colors.text, fontWeight: '900' },
  total: { fontSize: 22, fontWeight: '900' },
  divider: { backgroundColor: colors.line, height: 1 },
  process: { alignItems: 'center', flexDirection: 'row', gap: 10 },
});
