import { LinearGradient } from 'expo-linear-gradient';
import { useState } from 'react';
import { Alert, KeyboardAvoidingView, Platform, ScrollView, StyleSheet, Text, View } from 'react-native';
import { SafeAreaView } from 'react-native-safe-area-context';

import { useAuth } from '../auth/AuthContext';
import { BrandHeader, Button, Card, Field } from '../components/ui';
import { colors } from '../theme/colors';

export function RegisterScreen({ onLogin }: { onLogin(): void }) {
  const { signUp } = useAuth();
  const [login, setLogin] = useState('');
  const [password, setPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [names, setNames] = useState('');
  const [lastNames, setLastNames] = useState('');
  const [identificationType, setIdentificationType] = useState('CEDULA');
  const [identificationNumber, setIdentificationNumber] = useState('');
  const [phone, setPhone] = useState('');
  const [address, setAddress] = useState('');
  const [loading, setLoading] = useState(false);

  async function submit() {
    const validation = validate();
    if (validation) {
      Alert.alert('Revisa tus datos', validation);
      return;
    }

    setLoading(true);
    try {
      await signUp({
        login,
        password,
        names,
        lastNames,
        identificationType,
        identificationNumber,
        phone,
        address,
      });
    } catch (error) {
      Alert.alert('No se pudo crear la cuenta', error instanceof Error ? error.message : 'Intenta nuevamente.');
    } finally {
      setLoading(false);
    }
  }

  function validate() {
    if (!login.trim() || !login.includes('@')) return 'Ingresa un correo valido.';
    if (password.length < 8) return 'La contrasena debe tener al menos 8 caracteres.';
    if (password !== confirmPassword) return 'Las contrasenas no coinciden.';
    if (!names.trim()) return 'Ingresa tus nombres.';
    if (!lastNames.trim()) return 'Ingresa tus apellidos.';
    if (!identificationType.trim()) return 'Ingresa el tipo de identificacion.';
    if (!identificationNumber.trim()) return 'Ingresa tu numero de identificacion.';
    return '';
  }

  return (
    <SafeAreaView style={styles.safe}>
      <KeyboardAvoidingView behavior={Platform.OS === 'ios' ? 'padding' : 'height'} style={styles.flex}>
        <ScrollView contentContainerStyle={styles.page} keyboardShouldPersistTaps="handled">
          <BrandHeader subtitle="Marketplace movil" />
          <LinearGradient colors={[colors.primaryDark, colors.primaryStrong]} style={styles.hero}>
            <Text style={styles.heroTitle}>Crea tu cuenta cliente</Text>
            <Text style={styles.heroCopy}>Reserva atracciones y consulta tus comprobantes desde el celular.</Text>
          </LinearGradient>

          <Card>
            <Text style={styles.title}>Registro</Text>
            <Text style={styles.copy}>Usaremos estos datos para asociar tus reservas a tu perfil de cliente.</Text>

            <Field
              autoCapitalize="none"
              autoComplete="email"
              keyboardType="email-address"
              label="Correo"
              onChangeText={setLogin}
              value={login}
            />
            <Field
              autoCapitalize="none"
              autoComplete="password"
              label="Contrasena"
              onChangeText={setPassword}
              secureTextEntry
              value={password}
            />
            <Field
              autoCapitalize="none"
              autoComplete="password"
              label="Confirmar contrasena"
              onChangeText={setConfirmPassword}
              secureTextEntry
              value={confirmPassword}
            />
            <View style={styles.columns}>
              <Field label="Nombres" onChangeText={setNames} value={names} />
              <Field label="Apellidos" onChangeText={setLastNames} value={lastNames} />
            </View>
            <View style={styles.columns}>
              <Field
                autoCapitalize="characters"
                label="Tipo identificacion"
                onChangeText={setIdentificationType}
                value={identificationType}
              />
              <Field
                keyboardType="number-pad"
                label="Numero identificacion"
                onChangeText={setIdentificationNumber}
                value={identificationNumber}
              />
            </View>
            <Field keyboardType="phone-pad" label="Telefono" onChangeText={setPhone} value={phone} />
            <Field label="Direccion" onChangeText={setAddress} value={address} />

            <Button icon="person-add-outline" label="Crear cuenta" loading={loading} onPress={submit} />
            <Button label="Ya tengo cuenta" onPress={onLogin} variant="secondary" />
          </Card>
        </ScrollView>
      </KeyboardAvoidingView>
    </SafeAreaView>
  );
}

const styles = StyleSheet.create({
  safe: { backgroundColor: colors.background, flex: 1 },
  flex: { flex: 1 },
  page: { flexGrow: 1, gap: 18, padding: 20, paddingBottom: 32 },
  hero: { borderRadius: 8, gap: 12, minHeight: 160, justifyContent: 'center', padding: 24 },
  heroTitle: { color: colors.white, fontSize: 30, fontWeight: '900', lineHeight: 36 },
  heroCopy: { color: '#d9f3ef', fontSize: 16, lineHeight: 23 },
  title: { color: colors.text, fontSize: 23, fontWeight: '900' },
  copy: { color: colors.muted, fontSize: 14, lineHeight: 20 },
  columns: { gap: 12 },
});
