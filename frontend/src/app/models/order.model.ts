export interface Order {
  id?: number;
  productId: string;
  clientId: number;
  quantity: number;
  createdAt?: Date;
}