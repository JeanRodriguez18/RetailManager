using Caliburn.Micro;
using RMDesktopUI.Library.Api;
using RMDesktopUI.Library.Models;
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

        public SalesViewModel(IProductEndpoint productEndpoint)
        {
            _productEndPoint = productEndpoint;
        }

        protected override async void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            await LoadProducts();
        }

        private async Task LoadProducts()
        {
            var productsList = await _productEndPoint.GetAll();
            Products = new BindingList<ProductModel>(productsList);
        }

        private BindingList<ProductModel> _products;

		public BindingList<ProductModel> Products
		{
			get { return _products; }
			set 
			{
				_products = value;
				NotifyOfPropertyChange(() => Products);
			}
		}

        private ProductModel _selectedProducts;

        public ProductModel SelectedProducts
        {
            get { return _selectedProducts; }
            set 
            {
                _selectedProducts = value;
                NotifyOfPropertyChange(() => SelectedProducts);
                NotifyOfPropertyChange(() => CanAddToCart);
            }
        }


        private BindingList<CartItemModel> _cart = new BindingList<CartItemModel>();

		public BindingList<CartItemModel> Cart
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
                //Todo Replace with Caculation
                decimal subtotal = 0;

                foreach (var item in Cart)
                {
                    subtotal += (item.Product.RetailPrice * item.QuantityInCart);
                    
                }
                return subtotal.ToString("C");
			}
		}

        public string Tax
        {
            get
            {
                //Todo Replace with Caculation
                return "0.00";
            }
        }

        public string Total
        {
            get
            {
                //Todo Replace with Caculation
                return "0.00";
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

            CartItemModel existingItem = Cart.FirstOrDefault(x=> x.Product == SelectedProducts);

            if (existingItem != null)
            {
                existingItem.QuantityInCart += ItemQuantity;
                //Hack - there should be a better way of refreshing the cart display
                Cart.Remove(existingItem);
                Cart.Add(existingItem);
            }
            else
            {
                CartItemModel item = new CartItemModel
                {
                    Product = SelectedProducts,
                    QuantityInCart = ItemQuantity
                };
                Cart.Add(item);
            }
            SelectedProducts.QuantityInStock -= ItemQuantity;
            ItemQuantity = 1;
            NotifyOfPropertyChange(() => SubTotal);

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

		}

        public bool CanCheckOut
        {
            get
            {
                bool output = false;

                //make sure something is in the Cart


                return output;
            }
        }

        public void CkeckOut()
        {

        }

    }
}
