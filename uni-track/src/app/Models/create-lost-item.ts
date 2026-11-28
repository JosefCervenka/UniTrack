export interface CreateLostItem {
  name: string;
  description: string;
  lostLocation: string;
  statusId: number;
  image?: string | null;
}
