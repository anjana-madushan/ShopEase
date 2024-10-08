package com.sliit.shopease.activity;

import android.content.Intent;
import android.os.Bundle;
import android.widget.Button;
import android.widget.ImageButton;
import android.widget.ImageView;
import android.widget.TextView;

import androidx.activity.EdgeToEdge;
import androidx.appcompat.app.AppCompatActivity;
import androidx.core.graphics.Insets;
import androidx.core.view.ViewCompat;
import androidx.core.view.WindowInsetsCompat;

import com.bumptech.glide.Glide;
import com.sliit.shopease.R;
import com.sliit.shopease.helpers.DialogHelper;
import com.sliit.shopease.interfaces.NetworkCallback;
import com.sliit.shopease.models.Cart;
import com.sliit.shopease.models.Product;
import com.sliit.shopease.models.ShopEaseError;
import com.sliit.shopease.repository.ProductsRepository;

public class ProductActivity extends AppCompatActivity {
  private final ProductsRepository productsRepository = new ProductsRepository();

  private Cart cart;
  private Product product;
  private TextView prod_txt_name;
  private TextView prod_txt_price;
  private TextView prod_txt_category;
  private TextView prod_txt_stock;
  private TextView prod_txt_description;
  private ImageView prod_img;
  private Button btn_add_cart;

  private String productId;

  @Override
  protected void onCreate(Bundle savedInstanceState) {
    super.onCreate(savedInstanceState);
    EdgeToEdge.enable(this);
    setContentView(R.layout.activity_product);
    ViewCompat.setOnApplyWindowInsetsListener(findViewById(R.id.main), (v, insets) -> {
      Insets systemBars = insets.getInsets(WindowInsetsCompat.Type.systemBars());
      v.setPadding(systemBars.left, systemBars.top, systemBars.right, systemBars.bottom);
      return insets;
    });

    ImageButton btn_cart = findViewById(R.id.prod_btn_cart);

    //get product id from intent
    productId = getIntent().getStringExtra("productId");
    cart = new Cart(this);

    btn_add_cart = findViewById(R.id.prod_btn_add_cart);
    prod_txt_name = findViewById(R.id.prod_txt_name);
    prod_txt_price = findViewById(R.id.prod_txt_price);
    prod_txt_category = findViewById(R.id.prod_txt_category);
    prod_txt_stock = findViewById(R.id.prod_txt_stock);
    prod_txt_description = findViewById(R.id.prod_txt_description);
    prod_img = findViewById(R.id.prod_img);

    btn_add_cart.setOnClickListener(v -> addToCart());
    btn_cart.setOnClickListener(v -> goToCart());

    loadData();
  }

  private void goToCart(){
    Intent intent = new Intent(ProductActivity.this, ShoppingCartActivity.class);
    startActivity(intent);
  }

  private void loadData() {
    DialogHelper.showLoading(this, "Loading...");

    productsRepository.getProduct(this, productId, new NetworkCallback<Product>() {
      @Override
      public void onSuccess(Product response) {
        product = response;

        runOnUiThread(() -> {
          prod_txt_name.setText(response.getProductName());
          prod_txt_price.setText(response.getPriceString());
          prod_txt_category.setText(response.getCategory());
          prod_txt_stock.setText(getString(R.string.available, response.getStockLevel()));
          prod_txt_description.setText(response.getDescription());
          btn_add_cart.setText(getString(R.string.add_to_cart, cart.getProductCount(response)));

          final String imageUrl = response.getImageUrl();
          if (imageUrl != null) {
            Glide.with(ProductActivity.this).load(imageUrl).into(prod_img);
          } else {
            prod_img.setImageResource(R.drawable.product_placeholder);
          }
        });
        DialogHelper.hideLoading();
      }

      @Override
      public void onFailure(ShopEaseError error) {
        System.out.println(error.getMessage());
        runOnUiThread(() -> {
          DialogHelper.hideLoading();
          DialogHelper.showAlert(ProductActivity.this, "Error", error.getMessage());
        });
      }
    });
  }

  private void addToCart() {
    cart.addItem(product);
    btn_add_cart.setText(getString(R.string.add_to_cart, cart.getProductCount(product)));
  }
}