export const environment = {
  gatewayUrl:
    process.env.EXPO_PUBLIC_API_GATEWAY_URL?.replace(/\/$/, '') ??
    'https://api-gateway-travel-dreams.onrender.com',
};
