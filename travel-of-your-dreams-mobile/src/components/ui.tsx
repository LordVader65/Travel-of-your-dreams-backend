import { Ionicons } from '@expo/vector-icons';
import { PropsWithChildren, ReactNode } from 'react';
import {
  ActivityIndicator,
  KeyboardAvoidingView,
  Platform,
  Pressable,
  ScrollView,
  StyleSheet,
  Text,
  TextInput,
  TextInputProps,
  View,
  ViewStyle,
} from 'react-native';
import { SafeAreaView } from 'react-native-safe-area-context';

import { colors } from '../theme/colors';

export function Screen({ children, scroll = true }: PropsWithChildren<{ scroll?: boolean }>) {
  const content = scroll ? (
    <ScrollView contentContainerStyle={styles.screenContent} keyboardShouldPersistTaps="handled">
      {children}
    </ScrollView>
  ) : (
    <View style={[styles.screenContent, styles.flex]}>{children}</View>
  );
  return (
    <SafeAreaView edges={['top']} style={styles.safe}>
      <KeyboardAvoidingView behavior={Platform.OS === 'ios' ? 'padding' : 'height'} style={styles.flex}>
        {content}
      </KeyboardAvoidingView>
    </SafeAreaView>
  );
}

export function BrandHeader({ title, subtitle }: { title?: string; subtitle?: string }) {
  return (
    <View style={styles.brandHeader}>
      <View style={styles.brandMark}><Text style={styles.brandMarkText}>TOYD</Text></View>
      <View style={styles.flex}>
        <Text style={styles.brand}>{title || 'Travel of Your Dreams'}</Text>
        {subtitle ? <Text style={styles.subtitle}>{subtitle}</Text> : null}
      </View>
    </View>
  );
}

export function SectionTitle({ title, subtitle, action }: { title: string; subtitle?: string; action?: ReactNode }) {
  return (
    <View style={styles.sectionTitle}>
      <View style={styles.flex}>
        <Text style={styles.h1}>{title}</Text>
        {subtitle ? <Text style={styles.subtitle}>{subtitle}</Text> : null}
      </View>
      {action}
    </View>
  );
}

export function Card({ children, style }: PropsWithChildren<{ style?: ViewStyle | ViewStyle[] }>) {
  return <View style={[styles.card, style]}>{children}</View>;
}

export function Button({
  label,
  onPress,
  icon,
  variant = 'primary',
  disabled,
  loading,
}: {
  label: string;
  onPress(): void;
  icon?: keyof typeof Ionicons.glyphMap;
  variant?: 'primary' | 'secondary' | 'danger';
  disabled?: boolean;
  loading?: boolean;
}) {
  return (
    <Pressable
      accessibilityRole="button"
      disabled={disabled || loading}
      onPress={onPress}
      style={({ pressed }) => [
        styles.button,
        variant === 'secondary' && styles.buttonSecondary,
        variant === 'danger' && styles.buttonDanger,
        (disabled || loading) && styles.disabled,
        pressed && styles.pressed,
      ]}
    >
      {loading ? <ActivityIndicator color={variant === 'secondary' ? colors.text : colors.white} /> : null}
      {icon && !loading ? <Ionicons color={variant === 'secondary' ? colors.text : colors.white} name={icon} size={18} /> : null}
      <Text style={[styles.buttonText, variant === 'secondary' && styles.buttonTextSecondary]}>{label}</Text>
    </Pressable>
  );
}

export function Field({ label, error, ...props }: TextInputProps & { label: string; error?: string }) {
  return (
    <View style={styles.field}>
      <Text style={styles.label}>{label}</Text>
      <TextInput
        placeholderTextColor="#98a2b3"
        style={[styles.input, props.multiline && styles.multiline, error && styles.inputError]}
        {...props}
      />
      {error ? <Text style={styles.errorText}>{error}</Text> : null}
    </View>
  );
}

export function Chip({ label, active = false, onPress }: { label: string; active?: boolean; onPress?: () => void }) {
  const content = <Text style={[styles.chipText, active && styles.chipTextActive]}>{label}</Text>;
  if (!onPress) return <View style={[styles.chip, active && styles.chipActive]}>{content}</View>;
  return <Pressable onPress={onPress} style={[styles.chip, active && styles.chipActive]}>{content}</Pressable>;
}

export function StatusBadge({ status }: { status: string }) {
  const normalized = status.toUpperCase();
  const negative = ['CANCELADA', 'EXPIRADA', 'RECHAZADA', 'PAGO_RECHAZADO'].some((x) => normalized.includes(x));
  const pending = ['PENDIENTE', 'RECIBIDA', 'PROCESANDO'].some((x) => normalized.includes(x));
  return (
    <View style={[styles.status, negative && styles.statusNegative, pending && styles.statusPending]}>
      <Text style={[styles.statusText, negative && styles.statusTextNegative, pending && styles.statusTextPending]}>
        {status.replaceAll('_', ' ')}
      </Text>
    </View>
  );
}

export function LoadingState({ message = 'Cargando...' }: { message?: string }) {
  return <View style={styles.state}><ActivityIndicator color={colors.primary} /><Text style={styles.subtitle}>{message}</Text></View>;
}

export function EmptyState({ icon = 'search-outline', title, message }: { icon?: keyof typeof Ionicons.glyphMap; title?: string; message: string }) {
  return <View style={styles.state}><Ionicons color={colors.muted} name={icon} size={30} />{title ? <Text style={styles.emptyTitle}>{title}</Text> : null}<Text style={styles.subtitle}>{message}</Text></View>;
}

export function ErrorState({ message, retry }: { message: string; retry?: () => void }) {
  return <Card><View style={styles.state}><Ionicons color={colors.danger} name="alert-circle" size={30} /><Text style={styles.errorCentered}>{message}</Text>{retry ? <Button label="Reintentar" onPress={retry} variant="secondary" /> : null}</View></Card>;
}

const styles = StyleSheet.create({
  safe: { backgroundColor: colors.background, flex: 1 },
  flex: { flex: 1 },
  screenContent: { backgroundColor: colors.background, flexGrow: 1, gap: 16, padding: 18, paddingBottom: 32 },
  brandHeader: { alignItems: 'center', flexDirection: 'row', gap: 12 },
  brandMark: { alignItems: 'center', backgroundColor: colors.primaryDark, borderRadius: 8, height: 46, justifyContent: 'center', width: 54 },
  brandMarkText: { color: colors.white, fontSize: 14, fontWeight: '900' },
  brand: { color: colors.text, fontSize: 18, fontWeight: '900' },
  subtitle: { color: colors.muted, fontSize: 14, lineHeight: 20 },
  h1: { color: colors.text, fontSize: 27, fontWeight: '900', letterSpacing: 0 },
  sectionTitle: { alignItems: 'center', flexDirection: 'row', gap: 12, justifyContent: 'space-between' },
  card: { backgroundColor: colors.surface, borderColor: colors.line, borderRadius: 8, borderWidth: 1, gap: 12, padding: 16 },
  button: { alignItems: 'center', backgroundColor: colors.primary, borderRadius: 7, flexDirection: 'row', gap: 8, justifyContent: 'center', minHeight: 48, paddingHorizontal: 16 },
  buttonSecondary: { backgroundColor: colors.surfaceMuted },
  buttonDanger: { backgroundColor: colors.danger },
  buttonText: { color: colors.white, fontSize: 15, fontWeight: '800' },
  buttonTextSecondary: { color: colors.text },
  disabled: { opacity: 0.5 },
  pressed: { opacity: 0.85, transform: [{ translateY: 1 }] },
  field: { gap: 6 },
  label: { color: colors.muted, fontSize: 13, fontWeight: '800' },
  input: { backgroundColor: colors.surface, borderColor: colors.line, borderRadius: 7, borderWidth: 1, color: colors.text, fontSize: 16, minHeight: 48, paddingHorizontal: 12, paddingVertical: 10 },
  multiline: { minHeight: 90, textAlignVertical: 'top' },
  inputError: { borderColor: colors.danger },
  errorText: { color: colors.danger, fontSize: 12, fontWeight: '600' },
  errorCentered: { color: colors.danger, fontSize: 14, lineHeight: 20, textAlign: 'center' },
  emptyTitle: { color: colors.text, fontSize: 16, fontWeight: '800', textAlign: 'center' },
  chip: { alignSelf: 'flex-start', backgroundColor: colors.surfaceMuted, borderRadius: 999, paddingHorizontal: 11, paddingVertical: 8 },
  chipActive: { backgroundColor: colors.primary },
  chipText: { color: colors.text, fontSize: 12, fontWeight: '800' },
  chipTextActive: { color: colors.white },
  status: { alignSelf: 'flex-start', backgroundColor: colors.primarySoft, borderRadius: 999, paddingHorizontal: 10, paddingVertical: 6 },
  statusNegative: { backgroundColor: '#fee4e2' },
  statusPending: { backgroundColor: '#fef0c7' },
  statusText: { color: colors.primaryStrong, fontSize: 11, fontWeight: '900' },
  statusTextNegative: { color: colors.danger },
  statusTextPending: { color: colors.warning },
  state: { alignItems: 'center', gap: 12, justifyContent: 'center', minHeight: 150, padding: 16 },
});

export const commonStyles = styles;
