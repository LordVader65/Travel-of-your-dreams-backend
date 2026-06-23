import { Ionicons } from '@expo/vector-icons';
import { NativeStackScreenProps } from '@react-navigation/native-stack';
import { useEffect, useState } from 'react';
import { Image, StyleSheet, Text, View } from 'react-native';

import { useAuth } from '../auth/AuthContext';
import { Button, Card, Chip, EmptyState, ErrorState, LoadingState, Screen, StatusBadge } from '../components/ui';
import { RootStackParamList } from '../navigation/types';
import { colors } from '../theme/colors';
import { Attraction, Schedule, Ticket } from '../types/models';

type Props = NativeStackScreenProps<RootStackParamList, 'AttractionDetail'>;

export function AttractionDetailScreen({ route, navigation }: Props) {
  const { api } = useAuth();
  const [item, setItem] = useState<Attraction>(route.params.attraction);
  const [schedules, setSchedules] = useState<Schedule[]>([]);
  const [tickets, setTickets] = useState<Ticket[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    Promise.all([api.attraction(item.guid), api.schedules(item.guid), api.tickets(item.guid)])
      .then(([detail, scheduleData, ticketData]) => {
        setItem({ ...item, ...detail });
        setSchedules(scheduleData);
        setTickets(ticketData);
      })
      .catch((reason) => setError(reason instanceof Error ? reason.message : 'No se pudo cargar el detalle.'))
      .finally(() => setLoading(false));
  }, [api, item.guid]);

  return (
    <Screen>
      <Image source={{ uri: item.imageUrl }} style={styles.heroImage} />
      <View style={styles.heading}>
        <View style={styles.flex}>
          <Text style={styles.eyebrow}>{item.country}</Text>
          <Text style={styles.title}>{item.name}</Text>
          <Text style={styles.address}>{item.address || 'Punto de encuentro por confirmar'}</Text>
        </View>
        <StatusBadge status={item.available ? 'RESERVABLE' : 'NO DISPONIBLE'} />
      </View>
      {loading ? <LoadingState message="Consultando disponibilidad..." /> : null}
      {error ? <ErrorState message={error} /> : null}
      <Card>
        <Text style={styles.sectionTitle}>Resumen</Text>
        <Text style={styles.copy}>{item.description || 'Experiencia turística disponible para reservar.'}</Text>
        <View style={styles.chips}>
          {item.freeCancellation ? <Chip label="Cancelación gratis" /> : null}
          {item.skipLine ? <Chip label="Sin fila" /> : null}
          {item.durationMinutes ? <Chip label={`${item.durationMinutes} minutos`} /> : null}
          {item.includesTransport ? <Chip label="Incluye transporte" /> : null}
          {item.includesGuide ? <Chip label="Incluye guía" /> : null}
        </View>
      </Card>
      {(item.categories.length || item.languages.length || item.includes.length || item.excludes.length) ? (
        <Card>
          <Text style={styles.sectionTitle}>Características</Text>
          <Feature title="Categorías" values={item.categories} />
          <Feature title="Idiomas" values={item.languages} />
          <Feature title="Incluye" values={item.includes} />
          <Feature title="No incluye" values={item.excludes} />
        </Card>
      ) : null}
      <Card>
        <Text style={styles.sectionTitle}>Entradas</Text>
        {tickets.map((entry) => (
          <View key={entry.guid} style={styles.row}>
            <View style={styles.flex}><Text style={styles.rowTitle}>{entry.title}</Text><Text style={styles.address}>{entry.participantType}</Text></View>
            <Text style={styles.price}>{entry.price.toFixed(2)} {entry.currency}</Text>
          </View>
        ))}
        {!tickets.length && !loading ? <EmptyState icon="ticket-outline" message="No hay entradas activas." /> : null}
      </Card>
      <Card>
        <Text style={styles.sectionTitle}>Próximos horarios</Text>
        {schedules.slice(0, 4).map((entry) => (
          <View key={entry.guid} style={styles.row}>
            <View style={styles.flex}><Text style={styles.rowTitle}>{formatDate(entry.date)}</Text><Text style={styles.address}>{entry.startTime}{entry.endTime ? ` - ${entry.endTime}` : ''}</Text></View>
            <Text style={styles.slots}>{entry.availableSlots} cupos</Text>
          </View>
        ))}
        {!schedules.length && !loading ? <EmptyState icon="calendar-outline" message="No hay horarios próximos disponibles." /> : null}
      </Card>
      <Card style={styles.bookingCard}>
        <View style={styles.bookingHead}>
          <View><Text style={styles.priceLarge}>{item.price.toFixed(2)} USD</Text><Text style={styles.address}>precio referencial</Text></View>
          <Ionicons color={colors.primary} name="shield-checkmark" size={26} />
        </View>
        <Button
          disabled={!schedules.length || !tickets.length}
          icon="calendar-outline"
          label="Elegir horario"
          onPress={() => navigation.navigate('Checkout', { attraction: item, schedules, tickets })}
        />
      </Card>
    </Screen>
  );
}

function Feature({ title, values }: { title: string; values: string[] }) {
  if (!values.length) return null;
  return <View style={styles.feature}><Text style={styles.featureTitle}>{title}</Text><View style={styles.chips}>{values.map((x) => <Chip key={x} label={x} />)}</View></View>;
}

function formatDate(value: string) {
  if (!value) return 'Fecha por confirmar';
  return new Intl.DateTimeFormat('es-EC', { dateStyle: 'medium' }).format(new Date(`${value}T12:00:00`));
}

const styles = StyleSheet.create({
  flex: { flex: 1 },
  heroImage: { aspectRatio: 4 / 3, backgroundColor: colors.surfaceMuted, borderRadius: 8, width: '100%' },
  heading: { alignItems: 'flex-start', flexDirection: 'row', gap: 12 },
  eyebrow: { color: colors.accent, fontSize: 12, fontWeight: '900', textTransform: 'uppercase' },
  title: { color: colors.text, fontSize: 31, fontWeight: '900', lineHeight: 37 },
  address: { color: colors.muted, fontSize: 13, lineHeight: 19 },
  sectionTitle: { color: colors.text, fontSize: 20, fontWeight: '900' },
  copy: { color: colors.text, fontSize: 15, lineHeight: 23 },
  chips: { flexDirection: 'row', flexWrap: 'wrap', gap: 8 },
  feature: { gap: 8 },
  featureTitle: { color: colors.muted, fontSize: 13, fontWeight: '800' },
  row: { alignItems: 'center', borderTopColor: colors.line, borderTopWidth: 1, flexDirection: 'row', gap: 12, justifyContent: 'space-between', paddingTop: 12 },
  rowTitle: { color: colors.text, fontSize: 15, fontWeight: '800' },
  price: { color: colors.text, fontSize: 15, fontWeight: '900' },
  slots: { color: colors.primaryStrong, fontSize: 13, fontWeight: '900' },
  bookingCard: { borderColor: '#9adbd2' },
  bookingHead: { alignItems: 'center', flexDirection: 'row', justifyContent: 'space-between' },
  priceLarge: { color: colors.text, fontSize: 25, fontWeight: '900' },
});
