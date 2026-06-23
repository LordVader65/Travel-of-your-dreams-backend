import { useCallback, useState } from 'react';
import { Pressable, StyleSheet, Text, View } from 'react-native';
import { useFocusEffect, useNavigation } from '@react-navigation/native';
import type { NativeStackNavigationProp } from '@react-navigation/native-stack';

import { useAuth } from '../auth/AuthContext';
import {
  BrandHeader,
  Button,
  Card,
  EmptyState,
  ErrorState,
  LoadingState,
  Screen,
  StatusBadge,
} from '../components/ui';
import type { RootStackParamList } from '../navigation/types';
import { colors } from '../theme/colors';
import type { Invoice, Reservation } from '../types/models';

type Tab = 'reservations' | 'invoices';

export function ActivityScreen() {
  const { api } = useAuth();
  const navigation = useNavigation<NativeStackNavigationProp<RootStackParamList>>();
  const [tab, setTab] = useState<Tab>('reservations');
  const [reservations, setReservations] = useState<Reservation[]>([]);
  const [invoices, setInvoices] = useState<Invoice[]>([]);
  const [expandedGuid, setExpandedGuid] = useState<string>();
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  const load = useCallback(async () => {
    setLoading(true);
    setError('');
    try {
      const [reservationItems, invoiceItems] = await Promise.all([
        api.reservations(),
        api.invoices(),
      ]);
      const enrichedReservations = await enrichReservations(reservationItems, api);
      const enrichedInvoices = await enrichInvoices(invoiceItems, api);
      setReservations(enrichedReservations);
      setInvoices(enrichedInvoices);
    } catch (reason) {
      setError(reason instanceof Error ? reason.message : 'No se pudo cargar tu actividad.');
    } finally {
      setLoading(false);
    }
  }, [api]);

  useFocusEffect(
    useCallback(() => {
      void load();
    }, [load]),
  );

  return (
    <Screen>
      <BrandHeader title="Mi actividad" subtitle="Reservas y comprobantes en un solo lugar" />

      <View style={styles.tabs}>
        <TabButton active={tab === 'reservations'} label="Mis reservas" onPress={() => setTab('reservations')} />
        <TabButton active={tab === 'invoices'} label="Mis facturas" onPress={() => setTab('invoices')} />
      </View>

      {loading ? <LoadingState message="Consultando actividad..." /> : null}
      {!loading && error ? <ErrorState message={error} retry={() => void load()} /> : null}

      {!loading && !error && tab === 'reservations' && reservations.length === 0 ? (
        <EmptyState title="Todavia no tienes reservas" message="Explora las atracciones y elige tu proxima experiencia." />
      ) : null}

      {!loading && !error && tab === 'reservations'
        ? reservations.map((reservation) => {
            const expanded = expandedGuid === reservation.guid;
            return (
              <Card key={reservation.guid} style={styles.recordCard}>
                <View style={styles.recordHeader}>
                  <View style={styles.recordText}>
                    <Text style={styles.recordTitle}>{reservation.code || 'Reserva'}</Text>
                    <Text style={styles.recordSubtitle}>{reservation.attractionName || 'Atraccion reservada'}</Text>
                  </View>
                  <StatusBadge status={reservation.status} />
                </View>

                <View style={styles.summaryRow}>
                  <Text style={styles.muted}>{reservationScheduleText(reservation)}</Text>
                  <Text style={styles.amount}>{formatMoney(reservation.total, reservation.currency)}</Text>
                </View>

                <Button
                  label={expanded ? 'Ocultar detalles' : 'Detalles'}
                  variant="secondary"
                  onPress={() => setExpandedGuid(expanded ? undefined : reservation.guid)}
                />

                {expanded ? (
                  <View style={styles.details}>
                    <Detail label="Horario" value={reservationScheduleText(reservation)} />
                    <Detail label="Entradas" value={reservationDetailsText(reservation)} />
                    {reservation.subtotal !== undefined ? <Detail label="Subtotal" value={formatMoney(reservation.subtotal, reservation.currency)} /> : null}
                    {reservation.tax !== undefined ? <Detail label="IVA" value={formatMoney(reservation.tax, reservation.currency)} /> : null}
                    {reservation.status.toUpperCase() === 'PENDIENTE' ? (
                      <Button
                        label="Continuar al pago"
                        onPress={() => navigation.navigate('Payment', { reservation })}
                      />
                    ) : null}
                  </View>
                ) : null}
              </Card>
            );
          })
        : null}

      {!loading && !error && tab === 'invoices' && invoices.length === 0 ? (
        <EmptyState title="Sin facturas" message="Las facturas emitidas despues de un pago apareceran aqui." />
      ) : null}

      {!loading && !error && tab === 'invoices'
        ? invoices.map((invoice) => (
            <Card key={invoice.guid} style={styles.recordCard}>
              <View style={styles.recordHeader}>
                <View style={styles.recordText}>
                  <Text style={styles.recordTitle}>{invoice.number || 'Factura'}</Text>
                  <Text style={styles.recordSubtitle}>{formatDate(invoice.issuedAt)}</Text>
                </View>
                <StatusBadge status={invoice.status} />
              </View>
              <View style={styles.summaryRow}>
                <Text style={styles.muted}>{invoiceReservationText(invoice)}</Text>
                <Text style={styles.amount}>{formatMoney(invoice.total, invoice.currency)}</Text>
              </View>
              <Button
                label="Ver factura"
                variant="secondary"
                onPress={() => navigation.navigate('InvoiceDetail', { invoice })}
              />
            </Card>
          ))
        : null}
    </Screen>
  );
}

async function enrichReservations(reservations: Reservation[], api: ReturnType<typeof useAuth>['api']) {
  return Promise.all(
    reservations.map(async (item) => {
      if (item.date && item.startTime && item.details.length) return item;
      try {
        const detail = await api.reservation(item.guid);
        return { ...item, ...detail };
      } catch {
        return item;
      }
    }),
  );
}

async function enrichInvoices(invoices: Invoice[], api: ReturnType<typeof useAuth>['api']) {
  return Promise.all(
    invoices.map(async (item) => {
      let next = item;

      if (item.guid && (!item.issuedAt || !item.reservationGuid || !item.reservationCode)) {
        try {
          const detail = await api.invoice(item.guid);
          next = { ...item, ...detail };
        } catch {
          next = item;
        }
      }

      if (next.reservationGuid && !next.reservationCode) {
        try {
          const reservation = await api.reservation(next.reservationGuid);
          next = {
            ...next,
            reservationCode: reservation.code,
            reservationAttractionName: reservation.attractionName,
          };
        } catch {
          return next;
        }
      }

      return next;
    }),
  );
}

function TabButton({ active, label, onPress }: { active: boolean; label: string; onPress: () => void }) {
  return (
    <Pressable
      accessibilityRole="tab"
      accessibilityState={{ selected: active }}
      onPress={onPress}
      style={[styles.tab, active && styles.activeTab]}
    >
      <Text style={[styles.tabText, active && styles.activeTabText]}>{label}</Text>
    </Pressable>
  );
}

function Detail({ label, value }: { label: string; value?: string }) {
  return (
    <View style={styles.detailRow}>
      <Text style={styles.detailLabel}>{label}</Text>
      <Text style={styles.detailValue}>{value || '-'}</Text>
    </View>
  );
}

function formatMoney(value: number, currency = 'USD') {
  return `${value.toFixed(2)} ${currency}`;
}

function reservationScheduleText(reservation: Reservation) {
  const date = formatScheduleDate(reservation.date);
  const time = [cleanTime(reservation.startTime), cleanTime(reservation.endTime)].filter(Boolean).join(' - ');
  if (date && time) return `${date}, ${time}`;
  if (date) return date;
  if (time) return time;
  return 'Horario por confirmar';
}

function reservationDetailsText(reservation: Reservation) {
  if (!reservation.details.length) return 'Sin detalle de entradas';
  return reservation.details
    .map((item) => {
      const title = item.ticketTitulo ?? item.ticket_titulo ?? item.titulo ?? 'Entrada';
      const quantity = item.cantidad ?? item.quantity ?? 1;
      return `${quantity} x ${title}`;
    })
    .join(', ');
}

function invoiceReservationText(invoice: Invoice) {
  if (invoice.reservationCode && invoice.reservationAttractionName) {
    return `${invoice.reservationCode} - ${invoice.reservationAttractionName}`;
  }
  if (invoice.reservationCode) return `Reserva ${invoice.reservationCode}`;
  if (invoice.reservationGuid) return `Reserva ${invoice.reservationGuid}`;
  return 'Reserva sin identificar';
}

function formatDate(value?: string) {
  if (!value) return 'Fecha no disponible';
  const date = new Date(value);
  return Number.isNaN(date.getTime())
    ? value
    : new Intl.DateTimeFormat('es-EC', { dateStyle: 'medium', timeStyle: 'short' }).format(date);
}

function formatScheduleDate(value?: string) {
  if (!value) return '';
  const date = new Date(`${value}T12:00:00`);
  return Number.isNaN(date.getTime())
    ? value
    : new Intl.DateTimeFormat('es-EC', { dateStyle: 'medium' }).format(date);
}

function cleanTime(value?: string) {
  if (!value) return '';
  return value.split('.').at(0)?.slice(0, 5) ?? value;
}

const styles = StyleSheet.create({
  tabs: { flexDirection: 'row', gap: 8, marginBottom: 16 },
  tab: { flex: 1, minHeight: 44, alignItems: 'center', justifyContent: 'center', borderRadius: 8, backgroundColor: colors.surfaceMuted },
  activeTab: { backgroundColor: colors.primary },
  tabText: { color: colors.text, fontSize: 14, fontWeight: '700' },
  activeTabText: { color: colors.white },
  recordCard: { marginBottom: 12, gap: 12 },
  recordHeader: { flexDirection: 'row', alignItems: 'center', justifyContent: 'space-between', gap: 12 },
  recordText: { flex: 1, gap: 3 },
  recordTitle: { color: colors.text, fontSize: 16, fontWeight: '800' },
  recordSubtitle: { color: colors.muted, fontSize: 14 },
  summaryRow: { flexDirection: 'row', alignItems: 'center', justifyContent: 'space-between', gap: 12 },
  muted: { color: colors.muted, fontSize: 13, flex: 1 },
  amount: { color: colors.text, fontSize: 15, fontWeight: '800' },
  details: { borderTopWidth: 1, borderTopColor: colors.line, paddingTop: 12, gap: 10 },
  detailRow: { gap: 3 },
  detailLabel: { color: colors.muted, fontSize: 12, fontWeight: '700' },
  detailValue: { color: colors.text, fontSize: 14 },
});
