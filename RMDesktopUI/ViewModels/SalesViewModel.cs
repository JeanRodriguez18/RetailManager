using AutoMapper;
using Caliburn.Micro;
using RMDesktopUI.Library.Api;
using RMDesktopUI.Library.Helpers;
using RMDesktopUI.Library.Models;
using RMDesktopUI.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMDesktopUI.ViewModels
{
    public class SalesViewModel : Screen
    {
        IProductEndpoint _productEndPoint;
        IConfigHelper _configHelper;
        ISaleEndpoint _saleEndpoint;
        IMapper _mapper;
        public SalesViewModel(IProductEndpoint productEndpoint, IConfigHelper configHelper, ISaleEndpoint saleEndpoint, IMapper mapper)
        {
            _productEndPoint = productEndpoint;
            _saleEndpoint = saleEndpoint;
            _configHelper = configHelper;
            _mapper = mapper;
        }

        protected override async void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            await LoadProducts();
        }

        private async Task LoadProducts()
        {
            var productsList = await _productEndPoint.GetAll();
            var products = _mapper.Map<List<ProductDisplayModel>>(productsList);
            Products = new BindingList<ProductDisplayModel>(products);
        }

        private BindingList<ProductDisplayModel> _products;

		public BindingList<ProductDisplayModel> Products
		{
			get { return _products; }
			set 
			{
				_products = value;
				NotifyOfPropertyChange(() => Products);
			}
		}

        private ProductDisplayModel _selectedProducts;

        public ProductDisplayModel SelectedProducts
        {
            get { return _selectedProducts; }
            set 
            {
                _selectedProducts = value;
                NotifyOfPropertyChange(() => SelectedProducts);
                NotifyOfPropertyChange(() => CanAddToCart);
            }
        }


        private BindingList<CartItemDisplayModel> _cart = new BindingList<CartItemDisplayModel>();

		public BindingList<CartItemDisplayModel> Cart
		{
			get { return _cart; }
			set 
			{
				_cart = value; 
				NotifyOfPropertyChange(() => Cart);
			}
		}

		public string SubTotal
		{
			get 
			{
                return CalculateSubTotal().ToString("C");
			}
		}

        private decimal CalculateSubTotal()
        {
            //Todo Replace with Caculation
            decimal subtotal = 0;

            foreach (var item in Cart)
            {
                subtotal += (item.Product.RetailPrice * item.QuantityInCart);

            }
            return subtotal;
        }

        private decimal CaculateTax()
        {
            //Todo Replace with Caculation
            decimal taxAmount = 0;
            decimal taxRate = _configHelper.GetTaxRate() / 100;

            taxAmount = Cart.Where(x => x.Product.IsTaxable).Sum(x => x.Product.RetailPrice * x.QuantityInCart * taxRate);

            return taxAmount;
        }

        public string Tax
        {
            get
            {
                return CaculateTax().ToString("C"); ;
            }
        }

        

        public string Total
        {
            get
            {
                //Todo Replace with Caculation
                decimal total = CalculateSubTotal() + CaculateTax();
                return total.ToString("C");
            }
        }


        private int _itemQuantity = 1;
        private readonly IProductEndpoint productEndpoint;

        public int ItemQuantity
		{
			get { return _itemQuantity; }
			set 
			{ 
			
				_itemQuantity = value;
				NotifyOfPropertyChange(() => ItemQuantity);
                NotifyOfPropertyChange(() => CanAddToCart);
			}
		}

        public bool CanAddToCart
        {
            get
            {
                bool output = false;

                //make sure something is selected
                //Make sure there is an item quantity
                if (ItemQuantity > 0 && SelectedProducts?.QuantityInStock >= ItemQuantity)
                {
                    output = true;
                }

                return output;
            }
        }

        public void AddToCart()
		{

            CartItemDisplayModel existingItem = Cart.FirstOrDefault(x=> x.Product == SelectedProducts);

            if (existingItem != null)
            {
                existingItem.QuantityInCart += ItemQuantity;
                //Hack - there should be a better way of refreshing the cart display
                Cart.Remove(existingItem);
                Cart.Add(existingItem);
            }
            else
            {
                CartItemDisplayModel item = new CartItemDisplayModel
                {
                    Product = SelectedProducts,
                    QuantityInCart = ItemQuantity
                };
                Cart.Add(item);
            }
            SelectedProducts.QuantityInStock -= ItemQuantity;
            ItemQuantity = 1;
            NotifyOfPropertyChange(() => SubTotal);
            NotifyOfPropertyChange(() => Tax);
            NotifyOfPropertyChange(() => Total);
            NotifyOfPropertyChange(() => CanCheckOut);


        }

        public bool CanRemoveFromCart
        {
            get
            {
                bool output = false;

                //make sure something is selected


                return output;
            }
        }

		public void RemoveFromCart()
		{
            NotifyOfPropertyChange(() => SubTotal);
            NotifyOfPropertyChange(() => Tax);
            NotifyOfPropertyChange(() => Total);
            NotifyOfPropertyChange(() => CanCheckOut);

        }

        public bool CanCheckOut
        {
            get
            {
                bool output = false;

                //make sure something is in the Cart
                if (Cart.Count > 0)
                {
                    output = true;
                }

                return output;
            }
        }

        public async Task CheckOut()
        {
            //Create a SaleModel and post to the API
            SaleModel sale = new SaleModel();
            foreach (var item in Cart)
            {
                sale.SaleDetails.Add(new SaleDetailModel
                {
                    ProductId = item.Product.Id,
                    Quantity = item.QuantityInCart
                });

            }

            await _saleEndpoint.PostSale(sale);
        }

    }
}
