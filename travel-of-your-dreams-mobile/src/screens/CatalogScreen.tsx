import { Ionicons } from '@expo/vector-icons';
import { NativeStackNavigationProp } from '@react-navigation/native-stack';
import { useFocusEffect, useNavigation } from '@react-navigation/native';
import { useCallback, useMemo, useState } from 'react';
import { Image, Pressable, RefreshControl, ScrollView, StyleSheet, Text, TextInput, View } from 'react-native';
import { SafeAreaView } from 'react-native-safe-area-context';

import { useAuth } from '../auth/AuthContext';
import { BrandHeader, EmptyState, ErrorState, LoadingState, StatusBadge } from '../components/ui';
import { RootStackParamList } from '../navigation/types';
import { colors } from '../theme/colors';
import { Attraction } from '../types/models';

export function CatalogScreen() {
  const { api } = useAuth();
  const navigation = useNavigation<NativeStackNavigationProp<RootStackParamList>>();
  const [data, setData] = useState<Attraction[]>([]);
  const [search, setSearch] = useState('');
  const [loading, setLoading] = useState(true);
  const [refreshing, setRefreshing] = useState(false);
  const [error, setError] = useState('');

  const load = useCallback(async (refresh = false) => {
    refresh ? setRefreshing(true) : setLoading(true);
    setError('');
    try {
      setData(await api.attractions());
    } catch (reason) {
      setError(reason instanceof Error ? reason.message : 'No se pudo cargar el catálogo.');
    } finally {
      setLoading(false);
      setRefreshing(false);
    }
  }, [api]);

  useFocusEffect(useCallback(() => { void load(); }, [load]));

  const filtered = useMemo(() => {
    const term = normalizeSearch(search);
    if (!term) return data;
    return data.filter((x) => normalizeSearch([
      x.name,
      x.country,
      x.address,
      x.description,
      ...x.categories,
      ...x.languages,
      ...x.includes,
    ].join(' ')).includes(term));
  }, [data, search]);

  return (
    <SafeAreaView edges={['top']} style={styles.safe}>
      <ScrollView
        contentContainerStyle={styles.content}
        refreshControl={<RefreshControl onRefresh={() => load(true)} refreshing={refreshing} tintColor={colors.primary} />}
      >
        <BrandHeader subtitle="Experiencias con disponibilidad real" />
        <View style={styles.hero}>
          <Text style={styles.heroTitle}>Encuentra tu siguiente experiencia</Text>
          <Text style={styles.heroCopy}>Compara opciones, elige un horario y reserva en pocos pasos.</Text>
        </View>
        <View style={styles.searchBox}>
          <Ionicons color={colors.muted} name="search" size={20} />
          <TextInput onChangeText={setSearch} placeholder="Buscar atracción o destino" placeholderTextColor="#98a2b3" style={styles.search} value={search} />
        </View>
        {loading ? <LoadingState message="Buscando experiencias..." /> : null}
        {!loading && error ? <ErrorState message={error} retry={() => load()} /> : null}
        {!loading && !error && !filtered.length ? <EmptyState message="No encontramos experiencias con esa búsqueda." /> : null}
        {!loading && !error ? filtered.map((item) => (
          <Pressable key={item.guid} onPress={() => navigation.navigate('AttractionDetail', { attraction: item })} style={({ pressed }) => [styles.card, pressed && styles.pressed]}>
            <Image source={{ uri: item.imageUrl }} style={styles.image} />
            <View style={styles.cardBody}>
              <View style={styles.cardHead}>
                <Text style={styles.eyebrow}>{item.country}</Text>
                <StatusBadge status={item.available ? 'DISPONIBLE' : 'NO DISPONIBLE'} />
              </View>
              <Text style={styles.cardTitle}>{item.name}</Text>
              <Text numberOfLines={2} style={styles.description}>{item.description || 'Experiencia turística disponible para reservar.'}</Text>
              <View style={styles.meta}>
                <Text style={styles.price}>Desde {item.price.toFixed(2)} USD</Text>
                <Text style={styles.reviews}>{item.reviews} reseñas</Text>
              </View>
            </View>
          </Pressable>
        )) : null}
      </ScrollView>
    </SafeAreaView>
  );
}

function normalizeSearch(value: string) {
  return value
    .trim()
    .toLowerCase()
    .normalize('NFD')
    .replace(/[\u0300-\u036f]/g, '');
}

const styles = StyleSheet.create({
  safe: { backgroundColor: colors.background, flex: 1 },
  content: { gap: 16, padding: 18, paddingBottom: 32 },
  hero: { backgroundColor: colors.primaryDark, borderRadius: 8, gap: 10, minHeight: 210, justifyContent: 'center', padding: 24 },
  heroTitle: { color: colors.white, fontSize: 34, fontWeight: '900', lineHeight: 40 },
  heroCopy: { color: '#d9f3ef', fontSize: 16, lineHeight: 23 },
  searchBox: { alignItems: 'center', backgroundColor: colors.surface, borderColor: colors.line, borderRadius: 8, borderWidth: 1, flexDirection: 'row', gap: 10, paddingHorizontal: 14 },
  search: { color: colors.text, flex: 1, fontSize: 15, minHeight: 50 },
  card: { backgroundColor: colors.surface, borderColor: colors.line, borderRadius: 8, borderWidth: 1, overflow: 'hidden' },
  pressed: { opacity: 0.88, transform: [{ scale: 0.995 }] },
  image: { aspectRatio: 4 / 3, backgroundColor: colors.surfaceMuted, width: '100%' },
  cardBody: { gap: 11, padding: 16 },
  cardHead: { alignItems: 'center', flexDirection: 'row', justifyContent: 'space-between' },
  eyebrow: { color: colors.accent, fontSize: 12, fontWeight: '900', textTransform: 'uppercase' },
  cardTitle: { color: colors.text, fontSize: 21, fontWeight: '900' },
  description: { color: colors.muted, fontSize: 14, lineHeight: 21 },
  meta: { alignItems: 'center', flexDirection: 'row', justifyContent: 'space-between' },
  price: { color: colors.text, fontSize: 15, fontWeight: '900' },
  reviews: { color: colors.muted, fontSize: 12 },
});
