import { LinearGradient } from 'expo-linear-gradient';
import { useState } from 'react';
import { Alert, KeyboardAvoidingView, Platform, ScrollView, StyleSheet, Text } from 'react-native';
import { SafeAreaView } from 'react-native-safe-area-context';

import { useAuth } from '../auth/AuthContext';
import { BrandHeader, Button, Card, Field } from '../components/ui';
import { colors } from '../theme/colors';

export function LoginScreen({ onRegister }: { onRegister(): void }) {
  const { signIn } = useAuth();
  const [login, setLogin] = useState('');
  const [password, setPassword] = useState('');
  const [loading, setLoading] = useState(false);

  async function submit() {
    if (!login.trim() || !password) {
      Alert.alert('Datos incompletos', 'Ingresa tu usuario y contraseña.');
      return;
    }
    setLoading(true);
    try {
      await signIn(login, password);
    } catch (error) {
      Alert.alert('No se pudo iniciar sesión', error instanceof Error ? error.message : 'Intenta nuevamente.');
    } finally {
      setLoading(false);
    }
  }

  return (
    <SafeAreaView style={styles.safe}>
      <KeyboardAvoidingView behavior={Platform.OS === 'ios' ? 'padding' : 'height'} style={styles.flex}>
      <ScrollView contentContainerStyle={styles.page} keyboardShouldPersistTaps="handled">
        <BrandHeader subtitle="Marketplace móvil" />
        <LinearGradient colors={[colors.primaryDark, colors.primaryStrong]} style={styles.hero}>
          <Text style={styles.heroTitle}>Tu próxima experiencia empieza aquí</Text>
          <Text style={styles.heroCopy}>Atracciones, horarios reales y reservas seguras desde un solo lugar.</Text>
        </LinearGradient>
        <Card>
          <Text style={styles.title}>Bienvenido</Text>
          <Text style={styles.copy}>Inicia sesión con tu cuenta de cliente TravelDreams.</Text>
          <Field autoCapitalize="none" autoComplete="username" label="Usuario o correo" onChangeText={setLogin} value={login} />
          <Field autoCapitalize="none" autoComplete="password" label="Contraseña" onChangeText={setPassword} onSubmitEditing={submit} secureTextEntry value={password} />
          <Button icon="log-in-outline" label="Ingresar" loading={loading} onPress={submit} />
          <Button label="Crear cuenta" onPress={onRegister} variant="secondary" />
        </Card>
      </ScrollView>
      </KeyboardAvoidingView>
    </SafeAreaView>
  );
}

const styles = StyleSheet.create({
  safe: { backgroundColor: colors.background, flex: 1 },
  flex: { flex: 1 },
  page: { flexGrow: 1, gap: 20, justifyContent: 'center', padding: 20, paddingBottom: 32 },
  hero: { borderRadius: 8, gap: 12, minHeight: 190, justifyContent: 'center', padding: 24 },
  heroTitle: { color: colors.white, fontSize: 32, fontWeight: '900', lineHeight: 38 },
  heroCopy: { color: '#d9f3ef', fontSize: 16, lineHeight: 23 },
  title: { color: colors.text, fontSize: 23, fontWeight: '900' },
  copy: { color: colors.muted, fontSize: 14, lineHeight: 20 },
});
