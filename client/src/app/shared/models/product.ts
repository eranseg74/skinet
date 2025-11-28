// Two options - 1. type, 2. Interface. The behavior is the same
// Important to use camelCase notation because the response in the JSON format comming from the API is also in camelCase and the naming must be identical for the mapping between the response and the type to be executed correctly!

// type approach
export type Product = {
  id: number;
  name: string;
  description: string;
  price: number;
  pictureUrl: string;
  type: string;
  brand: string;
  quantityInStock: number;
};

// Interface approach
// export interface Product {
//   id: number;
//   name: string;
//   description: string;
//   price: number;
//   pictureUrl: string;
//   type: string;
//   brand: string;
//   quantityInStock: number;
// }
