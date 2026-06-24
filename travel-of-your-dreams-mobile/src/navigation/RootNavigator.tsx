import { Ionicons } from '@expo/vector-icons';
import { createBottomTabNavigator } from '@react-navigation/bottom-tabs';
import { createNativeStackNavigator } from '@react-navigation/native-stack';
import { useState } from 'react';
import { useSafeAreaInsets } from 'react-native-safe-area-context';

import { useAuth } from '../auth/AuthContext';
import { ActivityScreen } from '../screens/ActivityScreen';
import { AttractionDetailScreen } from '../screens/AttractionDetailScreen';
import { CatalogScreen } from '../screens/CatalogScreen';
import { CheckoutScreen } from '../screens/CheckoutScreen';
import { InvoiceDetailScreen } from '../screens/InvoiceDetailScreen';
import { LoginScreen } from '../screens/LoginScreen';
import { PaymentScreen } from '../screens/PaymentScreen';
import { ProfileScreen } from '../screens/ProfileScreen';
import { RegisterScreen } from '../screens/RegisterScreen';
import { colors } from '../theme/colors';
import { MainTabParamList, RootStackParamList } from './types';

const Root = createNativeStackNavigator<RootStackParamList>();
const Tabs = createBottomTabNavigator<MainTabParamList>();

function MainTabs() {
  const insets = useSafeAreaInsets();
  const bottomInset = Math.max(insets.bottom, 10);

  return (
    <Tabs.Navigator
      screenOptions={({ route }) => ({
        headerShown: false,
        tabBarActiveTintColor: colors.primary,
        tabBarInactiveTintColor: colors.muted,
        tabBarLabelStyle: { fontSize: 12, fontWeight: '700' },
        tabBarHideOnKeyboard: true,
        tabBarItemStyle: { paddingVertical: 4 },
        tabBarStyle: {
          borderTopColor: colors.line,
          height: 56 + bottomInset,
          paddingBottom: bottomInset,
          paddingTop: 6,
        },
        tabBarIcon: ({ color, size }) => (
          <Ionicons
            color={color}
            name={route.name === 'Explore' ? 'compass' : route.name === 'Activity' ? 'receipt' : 'person'}
            size={size}
          />
        ),
      })}
    >
      <Tabs.Screen name="Explore" component={CatalogScreen} options={{ title: 'Explorar' }} />
      <Tabs.Screen name="Activity" component={ActivityScreen} options={{ title: 'Mi actividad' }} />
      <Tabs.Screen name="Profile" component={ProfileScreen} options={{ title: 'Perfil' }} />
    </Tabs.Navigator>
  );
}

export function RootNavigator() {
  const { session } = useAuth();
  const [authMode, setAuthMode] = useState<'login' | 'register'>('login');

  if (!session) {
    return authMode === 'login'
      ? <LoginScreen onRegister={() => setAuthMode('register')} />
      : <RegisterScreen onLogin={() => setAuthMode('login')} />;
  }

  return (
    <Root.Navigator
      screenOptions={{
        contentStyle: { backgroundColor: colors.background },
        headerBackTitle: 'Volver',
        headerTintColor: colors.text,
        headerTitleStyle: { fontWeight: '800' },
      }}
    >
      <Root.Screen name="Main" component={MainTabs} options={{ headerShown: false }} />
      <Root.Screen name="AttractionDetail" component={AttractionDetailScreen} options={{ title: 'Experiencia' }} />
      <Root.Screen name="Checkout" component={CheckoutScreen} options={{ title: 'Tu reserva' }} />
      <Root.Screen name="Payment" component={PaymentScreen} options={{ title: 'Confirmar pago' }} />
      <Root.Screen name="InvoiceDetail" component={InvoiceDetailScreen} options={{ title: 'Factura' }} />
    </Root.Navigator>
  );
}
