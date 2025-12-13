// Be careful from typo mistakes! The names must match the names in the DeliveryMethod.cs entity. Note that the id parameter is derived from the base entity. Checking for typo mistakes here is important because neither the compiler nor the application will issue warnings or exceptions in case of misspelling. It just won't work!
export type DeliveryMethod = {
  shortName: string;
  deliveryTime: string;
  description: string;
  price: number;
  id: number;
};
