// Using a class and not aa type to define the shop params in order to give it initial values
export class ShopParams {
  brands: string[] = [];
  types: string[] = [];
  sort = 'name';
  pageNumber = 1;
  pageSize = 10;
  search = '';
}
