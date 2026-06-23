import { Alert, StyleSheet, Text, View } from 'react-native';
import { useEffect, useState } from 'react';

import { useAuth } from '../auth/AuthContext';
import { BrandHeader, Button, Card, ErrorState, LoadingState, Screen } from '../components/ui';
import { colors } from '../theme/colors';
import type { CustomerProfile } from '../types/models';

export function ProfileScreen() {
  const { api, session, signOut } = useAuth();
  const [profile, setProfile] = useState<CustomerProfile | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    let mounted = true;

    async function load() {
      setLoading(true);
      setError('');
      try {
        const data = await api.profile(session?.login);
        if (mounted) setProfile(data);
      } catch (reason) {
        if (mounted) setError(reason instanceof Error ? reason.message : 'No se pudo cargar tu perfil.');
      } finally {
        if (mounted) setLoading(false);
      }
    }

    void load();
    return () => {
      mounted = false;
    };
  }, [api, session?.login]);

  const confirmSignOut = () => {
    Alert.alert('Cerrar sesion', 'Tendras que volver a ingresar tus credenciales.', [
      { text: 'Cancelar', style: 'cancel' },
      { text: 'Salir', style: 'destructive', onPress: () => void signOut() },
    ]);
  };

  return (
    <Screen>
      <BrandHeader title="Mi perfil" subtitle="Cuenta de cliente TravelDreams" />
      {loading ? <LoadingState message="Cargando perfil..." /> : null}
      {!loading && error ? <ErrorState message={error} /> : null}
      <Card style={styles.profileCard}>
        <View style={styles.avatar}>
          <Text style={styles.avatarText}>{initials(profile?.displayName || session?.login)}</Text>
        </View>
        <Text style={styles.name}>{profile?.displayName || session?.login || 'Cliente'}</Text>
        <Text style={styles.login}>{profile?.email || session?.login}</Text>
      </Card>

      <Card style={styles.detailsCard}>
        <ProfileRow label="Nombres" value={profile?.names} />
        <ProfileRow label="Apellidos" value={profile?.lastNames} />
        <ProfileRow label="Razon social" value={profile?.businessName} />
        <ProfileRow label="Correo" value={profile?.email || session?.login} />
        <ProfileRow label="Telefono" value={profile?.phone} />
        <ProfileRow label="Direccion" value={profile?.address} />
        <ProfileRow label="Identificacion" value={formatIdentification(profile)} />
      </Card>

      <Button label="Cerrar sesion" variant="danger" onPress={confirmSignOut} />
    </Screen>
  );
}

function ProfileRow({ label, value }: { label: string; value?: string }) {
  if (!value) return null;
  return (
    <View style={styles.row}>
      <Text style={styles.label}>{label}</Text>
      <Text style={styles.value}>{value}</Text>
    </View>
  );
}

function initials(value?: string) {
  if (!value) return 'TD';
  return value.split(/\s+/).slice(0, 2).map((part) => part[0]?.toUpperCase()).join('');
}

function formatIdentification(profile: CustomerProfile | null) {
  if (!profile?.identificationNumber) return undefined;
  return [profile.identificationType, profile.identificationNumber].filter(Boolean).join(' ');
}

const styles = StyleSheet.create({
  profileCard: { alignItems: 'center', gap: 7, marginBottom: 12 },
  avatar: { width: 78, height: 78, borderRadius: 39, alignItems: 'center', justifyContent: 'center', backgroundColor: colors.primaryDark, marginBottom: 4 },
  avatarText: { color: colors.white, fontSize: 24, fontWeight: '900' },
  name: { color: colors.text, fontSize: 19, fontWeight: '900' },
  login: { color: colors.muted, fontSize: 14 },
  detailsCard: { gap: 14, marginBottom: 16 },
  row: { gap: 4, paddingBottom: 12, borderBottomWidth: 1, borderBottomColor: colors.line },
  label: { color: colors.muted, fontSize: 12, fontWeight: '700' },
  value: { color: colors.text, fontSize: 14 },
});
