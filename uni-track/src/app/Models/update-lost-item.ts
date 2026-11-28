export interface UpdateLostItem {
  id: string;
  name: string;
  description: string;
  lostLocation: string;
  statusId: number;
  image?: string | null;
}
