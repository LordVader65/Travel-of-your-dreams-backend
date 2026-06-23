import { Attraction, Invoice, Reservation, Schedule, Ticket } from '../types/models';

export type RootStackParamList = {
  Main: undefined;
  AttractionDetail: { attraction: Attraction };
  Checkout: { attraction: Attraction; schedules: Schedule[]; tickets: Ticket[] };
  Payment: { reservation: Reservation };
  InvoiceDetail: { invoice: Invoice };
};

export type MainTabParamList = {
  Explore: undefined;
  Activity: undefined;
  Profile: undefined;
};
