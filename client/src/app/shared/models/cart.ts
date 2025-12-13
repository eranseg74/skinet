import { nanoid } from 'nanoid';

export type CartType = {
  id: string;
  items: CartItem[];
  deliveryMethodId?: number;
  paymentIntentId?: string;
  clientSecret?: string;
};

export type CartItem = {
  productId: number;
  productName: string;
  price: number;
  quantity: number;
  pictureUrl: string;
  brand: string;
  type: string;
};

export class Cart implements CartType {
  // nanoid is a third party app that generates random id. Not in a level of GUID but good enough for cart id. The chance of generating two same Ids is 1:100000000 and even if 2 people get the same Id one of them will see the other items so no danger there
  id: string = nanoid();
  items: CartItem[] = [];
  deliveryMethodId?: number;
  paymentIntentId?: string;
  clientSecret?: string;
}
