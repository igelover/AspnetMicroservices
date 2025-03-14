﻿using System;
using System.Linq;
using System.Threading.Tasks;
using AspnetRunBasics.Models;
using AspnetRunBasics.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AspnetRunBasics
{
    public class CartModel : PageModel
    {
        private readonly IBasketService _basketService;

        public CartModel(IBasketService basketService)
        {
            _basketService = basketService ?? throw new ArgumentNullException(nameof(basketService));
        }

        public BasketModel Cart { get; set; } = new BasketModel();        

        public async Task<IActionResult> OnGetAsync()
        {
            var userName = "igelover";
            Cart = await _basketService.GetBasket(userName);

            return Page();
        }

        public async Task<IActionResult> OnPostRemoveToCartAsync(string productId)
        {
            var userName = "igelover";
            var basket = await _basketService.GetBasket(userName);

            // TODO: This will throw an error if more than one instance of a product is in cart.
            var item = basket.Items.Single(x => x.ProductId == productId);
            basket.Items.Remove(item);

            return RedirectToPage();
        }
    }
}