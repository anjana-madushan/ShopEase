export const navKeys = [
  {
    key: 'dashboard',
    name: 'Dashboard',
    icon: 'FaHome',
    subPages: []
  },
  {
    key: 'users',
    name: 'Users',
    icon: 'FaUsers',
    subPages: [
      { key: 'admins', name: 'Admins' },
      { key: 'csr', name: 'CSR' }
    ]
  },
  {
    key: 'orders',
    name: 'Orders',
    icon: 'FaShoppingCart',
    subPages: [] // No subpages, orders main page
  },
  {
    key: 'shipments',
    name: 'Shipments',
    icon: 'FaTruck',
    subPages: [] // No subpages, shipments main page
  },
  {
    key: 'inventory',
    name: 'Inventory',
    icon: 'FaBoxes',
    subPages: [] // No subpages, inventory main page
  },
  {
    key: 'venders',
    name: 'Vendors',
    icon: 'FaIndustry',
    subPages: [
      { key: 'addvenders', name: 'Add Vendors' },  
    ] 
  },
  {
    key: 'customers',
    name: 'Customers',
    icon: 'FaEye',
    subPages: [] // Views main page
  },
  {
    key: 'products',
    name: 'Products',
    icon: 'FaBox',
    subPages: [] // Products main page
  },
  {
    key: 'samples',
    name: 'Samples',
    icon: 'FaClipboard',
    subPages: [] // Samples main page
  },
  {
    key: 'components',
    name: 'Components',
    icon: 'FaCog',
    subPages: [
      { key: 'fabrics', name: 'Fabrics' },
      { key: 'labels', name: 'Labels' },
      { key: 'trims', name: 'Trims' },
      { key: 'packaging', name: 'Packaging' }
    ]
  },
  {
    key: 'libraries',
    name: 'Libraries',
    icon: 'FaBook',
    subPages: [
      { key: 'sizes', name: 'Sizes' },
      { key: 'dimensions', name: 'Dimensions' },
      { key: 'colors', name: 'Colors' }
    ]
  }
];
