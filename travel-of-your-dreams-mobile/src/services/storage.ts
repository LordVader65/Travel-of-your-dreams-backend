import * as SecureStore from 'expo-secure-store';
import { Platform } from 'react-native';

const sessionKey = 'toyd_mobile_session';

export async function readSessionValue() {
  if (Platform.OS === 'web') return globalThis.localStorage?.getItem(sessionKey) ?? null;
  return SecureStore.getItemAsync(sessionKey);
}

export async function writeSessionValue(value: string) {
  if (Platform.OS === 'web') {
    globalThis.localStorage?.setItem(sessionKey, value);
    return;
  }
  await SecureStore.setItemAsync(sessionKey, value);
}

export async function clearSessionValue() {
  if (Platform.OS === 'web') {
    globalThis.localStorage?.removeItem(sessionKey);
    return;
  }
  await SecureStore.deleteItemAsync(sessionKey);
}
