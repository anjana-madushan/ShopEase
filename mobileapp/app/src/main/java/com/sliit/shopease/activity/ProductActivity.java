package com.sliit.shopease.activity;

import android.os.Bundle;
import android.widget.Button;
import android.widget.ImageView;
import android.widget.TextView;

import androidx.activity.EdgeToEdge;
import androidx.appcompat.app.AppCompatActivity;
import androidx.core.graphics.Insets;
import androidx.core.view.ViewCompat;
import androidx.core.view.WindowInsetsCompat;

import com.sliit.shopease.R;
import com.sliit.shopease.helpers.DialogHelper;
import com.sliit.shopease.interfaces.NetworkCallback;
import com.sliit.shopease.models.Product;
import com.sliit.shopease.models.ShopEaseError;
import com.sliit.shopease.repository.ProductsRepository;

public class ProductActivity extends AppCompatActivity {
  private final ProductsRepository productsRepository = new ProductsRepository();

  private TextView prod_txt_name;
  private TextView prod_txt_price;
  private TextView prod_txt_category;
  private TextView prod_txt_stock;
  private TextView prod_txt_description;
  private ImageView prod_img;

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

    //get product id from intent
    productId = getIntent().getStringExtra("productId");

    Button btn_add_cart = findViewById(R.id.prod_btn_add_cart);

    prod_txt_name = findViewById(R.id.prod_txt_name);
    prod_txt_price = findViewById(R.id.prod_txt_price);
    prod_txt_category = findViewById(R.id.prod_txt_category);
    prod_txt_stock = findViewById(R.id.prod_txt_stock);
    prod_txt_description = findViewById(R.id.prod_txt_description);
    prod_img = findViewById(R.id.prod_img);

    btn_add_cart.setOnClickListener(v -> addToCart());

    loadData();
  }

  private void loadData() {
    DialogHelper.showLoading(this, "Loading...");

    productsRepository.getProduct(this, productId, new NetworkCallback<Product>() {
      @Override
      public void onSuccess(Product response) {
        runOnUiThread(() -> {
          prod_txt_name.setText(response.getProductName());
          prod_txt_price.setText(response.getPriceString());
          prod_txt_category.setText(response.getCategory());
          prod_txt_stock.setText(getString(R.string.available, response.getStockLevel()));
          prod_txt_description.setText(response.getDescription());
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
    //TODO: Add to cart
  }
}