import { StyleSheet, Text, View } from 'react-native';
import type { NativeStackScreenProps } from '@react-navigation/native-stack';
import { useEffect, useMemo, useState } from 'react';

import { useAuth } from '../auth/AuthContext';
import { BrandHeader, Button, Card, ErrorState, LoadingState, Screen } from '../components/ui';
import type { RootStackParamList } from '../navigation/types';
import { colors } from '../theme/colors';
import type { Invoice, Reservation } from '../types/models';

type Props = NativeStackScreenProps<RootStackParamList, 'InvoiceDetail'>;

export function InvoiceDetailScreen({ route, navigation }: Props) {
  const { api } = useAuth();
  const [invoice, setInvoice] = useState<Invoice>(route.params.invoice);
  const [reservation, setReservation] = useState<Reservation | null>(null);
  const [loading, setLoading] = useState(Boolean(route.params.invoice.guid));
  const [error, setError] = useState('');
  const subtotal = invoice.subtotal ?? Math.max(invoice.total - (invoice.tax ?? 0), 0);
  const tax = invoice.tax ?? Math.max(invoice.total - subtotal, 0);
  const reservationLabel = useMemo(
    () => buildReservationLabel(invoice, reservation),
    [invoice, reservation],
  );

  useEffect(() => {
    let mounted = true;

    async function load() {
      if (!route.params.invoice.guid) return;
      setLoading(true);
      setError('');
      try {
        const detail = await api.invoice(route.params.invoice.guid);
        if (!mounted) return;
        const merged = { ...route.params.invoice, ...detail };
        setInvoice(merged);

        if (merged.reservationGuid) {
          try {
            const reservationDetail = await api.reservation(merged.reservationGuid);
            if (mounted) setReservation(reservationDetail);
          } catch {
            if (mounted) setReservation(null);
          }
        }
      } catch (reason) {
        if (mounted) setError(reason instanceof Error ? reason.message : 'No se pudo cargar la factura.');
      } finally {
        if (mounted) setLoading(false);
      }
    }

    void load();
    return () => {
      mounted = false;
    };
  }, [api, route.params.invoice]);

  return (
    <Screen>
      <BrandHeader title="Factura" subtitle={invoice.number} />
      {loading ? <LoadingState message="Cargando detalle de factura..." /> : null}
      {error ? <ErrorState message={error} /> : null}
      <Card style={styles.document}>
        <View style={styles.companyRow}>
          <View>
            <Text style={styles.company}>Travel of Your Dreams</Text>
            <Text style={styles.muted}>Servicios turisticos y reservas</Text>
            <Text style={styles.muted}>Quito, Ecuador</Text>
          </View>
          <View style={styles.logo}><Text style={styles.logoText}>TOYD</Text></View>
        </View>

        <View style={styles.divider} />
        <InvoiceRow label="Numero" value={invoice.number} />
        <InvoiceRow label="Fecha de emision" value={formatDate(invoice.issuedAt)} />
        <InvoiceRow label="Reserva" value={reservationLabel} />
        {reservation ? <InvoiceRow label="Horario" value={formatSchedule(reservation)} /> : null}
        <InvoiceRow label="Estado" value={invoice.status || 'A'} />
        <View style={styles.divider} />
        <InvoiceRow label="Subtotal" value={formatMoney(subtotal, invoice.currency)} />
        <InvoiceRow label="IVA" value={formatMoney(tax, invoice.currency)} />
        <View style={styles.totalRow}>
          <Text style={styles.totalLabel}>Total</Text>
          <Text style={styles.total}>{formatMoney(invoice.total, invoice.currency)}</Text>
        </View>
      </Card>
      <Button label="Cerrar" variant="secondary" onPress={() => navigation.goBack()} />
    </Screen>
  );
}

function buildReservationLabel(invoice: Invoice, reservation: Reservation | null) {
  const code = reservation?.code || invoice.reservationCode;
  const attraction = reservation?.attractionName;
  if (code && attraction) return `${code} - ${attraction}`;
  if (attraction) return attraction;
  if (code) return code;
  return invoice.reservationGuid || '-';
}

function InvoiceRow({ label, value }: { label: string; value: string }) {
  return (
    <View style={styles.row}>
      <Text style={styles.label}>{label}</Text>
      <Text style={styles.value}>{value}</Text>
    </View>
  );
}

function formatMoney(value: number, currency = 'USD') {
  return `${value.toFixed(2)} ${currency}`;
}

function formatDate(value?: string) {
  if (!value) return '-';
  const date = new Date(value);
  return Number.isNaN(date.getTime()) ? value : new Intl.DateTimeFormat('es-EC', { dateStyle: 'long' }).format(date);
}

function formatSchedule(reservation: Reservation) {
  const date = formatDate(reservation.date);
  const time = [reservation.startTime, reservation.endTime].filter(Boolean).join(' - ');
  return time ? `${date} ${time}` : date;
}

const styles = StyleSheet.create({
  document: { padding: 20, gap: 14, marginBottom: 16 },
  companyRow: { flexDirection: 'row', justifyContent: 'space-between', alignItems: 'center', gap: 16 },
  company: { color: colors.text, fontSize: 18, fontWeight: '900', marginBottom: 4 },
  muted: { color: colors.muted, fontSize: 13, marginTop: 2 },
  logo: { width: 64, height: 64, borderRadius: 32, alignItems: 'center', justifyContent: 'center', backgroundColor: colors.primaryDark },
  logoText: { color: colors.white, fontSize: 16, fontWeight: '900' },
  divider: { height: 1, backgroundColor: colors.line },
  row: { flexDirection: 'row', justifyContent: 'space-between', gap: 16 },
  label: { color: colors.muted, fontSize: 13, fontWeight: '700' },
  value: { color: colors.text, fontSize: 14, fontWeight: '600', flex: 1, textAlign: 'right' },
  totalRow: { flexDirection: 'row', justifyContent: 'space-between', alignItems: 'center', backgroundColor: colors.surfaceMuted, padding: 14, borderRadius: 8 },
  totalLabel: { color: colors.text, fontSize: 16, fontWeight: '800' },
  total: { color: colors.primary, fontSize: 20, fontWeight: '900' },
});
