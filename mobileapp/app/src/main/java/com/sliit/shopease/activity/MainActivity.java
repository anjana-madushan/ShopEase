package com.sliit.shopease.activity;

import android.content.Intent;
import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ImageButton;
import android.widget.ImageView;
import android.widget.TextView;

import androidx.activity.EdgeToEdge;
import androidx.annotation.NonNull;
import androidx.appcompat.app.AppCompatActivity;
import androidx.cardview.widget.CardView;
import androidx.core.graphics.Insets;
import androidx.core.view.ViewCompat;
import androidx.core.view.WindowInsetsCompat;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import com.bumptech.glide.Glide;
import com.sliit.shopease.R;
import com.sliit.shopease.constants.PrefKeys;
import com.sliit.shopease.helpers.DialogHelper;
import com.sliit.shopease.helpers.SharedPreferencesHelper;
import com.sliit.shopease.interfaces.NetworkCallback;
import com.sliit.shopease.models.Category;
import com.sliit.shopease.models.Product;
import com.sliit.shopease.models.ShopEaseError;
import com.sliit.shopease.repository.ProductsRepository;

import java.util.ArrayList;
import java.util.Collections;

public class MainActivity extends AppCompatActivity {
  private final ProductsRepository productsRepository = new ProductsRepository();

 private  RecyclerView rv_categories;
 private  RecyclerView rv_products;

  LinearLayoutManager h_linearLayoutManager;
  LinearLayoutManager v_linearLayoutManager;
  RvCategoriesAdapter rvCategoriesAdapter;
  RvProductsAdapter rvProductsAdapter;

  @Override
  protected void onCreate(Bundle savedInstanceState) {
    super.onCreate(savedInstanceState);

    if(!isUserLoggedIn()){
      return;
    }

    EdgeToEdge.enable(this);
    setContentView(R.layout.activity_main);
    ViewCompat.setOnApplyWindowInsetsListener(findViewById(R.id.main), (v, insets) -> {
      Insets systemBarsInsets = insets.getInsets(WindowInsetsCompat.Type.systemBars());

      // Apply system bar insets (status and navigation bar)
      v.setPadding(systemBarsInsets.left, systemBarsInsets.top, systemBarsInsets.right, 0);

      // Handle the keyboard (IME) inset
      Insets imeInsetsType = insets.getInsets(WindowInsetsCompat.Type.ime());
      if (insets.isVisible(WindowInsetsCompat.Type.ime())) {
        // Keyboard is visible, add padding to avoid overlapping
        v.setPadding(systemBarsInsets.left, systemBarsInsets.top, systemBarsInsets.right, imeInsetsType.bottom);
      }

      return insets;
    });

    ImageButton btn_profile = findViewById(R.id.btn_profile);
    ImageButton main_btn_cart = findViewById(R.id.main_btn_cart);

    rv_categories = findViewById(R.id.rv_categories);
    rv_products = findViewById(R.id.rv_items);

    btn_profile.setOnClickListener(v -> goToProfile());
    main_btn_cart.setOnClickListener(v-> goToCart());

    h_linearLayoutManager = new LinearLayoutManager(MainActivity.this, LinearLayoutManager.HORIZONTAL, false);
    v_linearLayoutManager = new LinearLayoutManager(MainActivity.this, LinearLayoutManager.VERTICAL, false);

    loadData();
  }

  private void checkUIReady() {
    if (rv_categories != null && rv_products != null) {
      DialogHelper.hideLoading();
    }
  }

  private void loadData() {
    DialogHelper.showLoading(MainActivity.this, "Please wait...");

    productsRepository.getAllProducts(MainActivity.this, new NetworkCallback<ArrayList<Product>>() {
      @Override
      public void onSuccess(ArrayList<Product> response) {
        runOnUiThread(() -> {
          rvProductsAdapter = new RvProductsAdapter(response);
          rv_products.setLayoutManager(v_linearLayoutManager);
          rv_products.setAdapter(rvProductsAdapter);
        });
        checkUIReady();
      }

      @Override
      public void onFailure(ShopEaseError error) {
        DialogHelper.hideLoading();
        runOnUiThread(() -> DialogHelper.showAlert(MainActivity.this, "Error", error.getMessage()));
      }
    });

    productsRepository.getAllCategories(MainActivity.this, new NetworkCallback<ArrayList<Category>>() {
      @Override
      public void onSuccess(ArrayList<Category> response) {
        runOnUiThread(() -> {
          rvCategoriesAdapter = new RvCategoriesAdapter(response);
          rv_categories.setLayoutManager(h_linearLayoutManager);
          rv_categories.setAdapter(rvCategoriesAdapter);
        });
        checkUIReady();
      }

      @Override
      public void onFailure(ShopEaseError error) {
        DialogHelper.hideLoading();
        runOnUiThread(() -> DialogHelper.showAlert(MainActivity.this, "Error", error.getMessage()));
      }
    });
  }

  private void goToProfile() {
    Intent intent = new Intent(MainActivity.this, ProfileActivity.class);
    startActivity(intent);
  }

  private void goToCart() {
    Intent intent = new Intent(MainActivity.this, ShoppingCartActivity.class);
    startActivity(intent);
  }

  private boolean isUserLoggedIn() {
    SharedPreferencesHelper sharedPreferencesHelper = new SharedPreferencesHelper(MainActivity.this);
    String token = sharedPreferencesHelper.getString(PrefKeys.USER, "");

    if (token.isEmpty()) {
      Intent intent = new Intent(MainActivity.this, SignInActivity.class);
      startActivity(intent);
      finish();
    }
    return !token.isEmpty();
  }

  public class RvCategoriesAdapter extends RecyclerView.Adapter<RvCategoriesAdapter.RvCategoryHolder> {
    private final ArrayList<Category> data;

    public RvCategoriesAdapter(ArrayList<Category> data) {
      this.data = data;
    }

    @NonNull
    @Override
    public RvCategoryHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
      View view = LayoutInflater.from(MainActivity.this).inflate(R.layout.layout_item_square, parent, false);
      return new RvCategoryHolder(view);
    }

    @Override
    public int getItemCount() {
      return data.size();
    }

    @Override
    public void onBindViewHolder(@NonNull RvCategoryHolder holder, int position) {
      holder.txt_categoryName.setText(data.get(position).getName());

      if (data.get(position).getImageUrl() != null) {
        Glide.with(MainActivity.this).load(data.get(position).getImageUrl()).into(holder.img_categoryImage);
      } else {
        holder.img_categoryImage.setImageResource(R.drawable.category_placeholder);
      }
    }

    class RvCategoryHolder extends RecyclerView.ViewHolder {
      private final TextView txt_categoryName;
      private final ImageView img_categoryImage;

      public RvCategoryHolder(@NonNull View itemView) {
        super(itemView);

        txt_categoryName = itemView.findViewById(R.id.sq_txt_item_label);
        img_categoryImage = itemView.findViewById(R.id.sq_img_item);
      }
    }
  }

  public class RvProductsAdapter extends RecyclerView.Adapter<RvProductsAdapter.RvProductHolder> {
    private final ArrayList<Product> data;

    public RvProductsAdapter(ArrayList<Product> data) {
      this.data = data;
      Collections.shuffle(this.data);
    }

    @NonNull
    @Override
    public RvProductHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
      View view = LayoutInflater.from(MainActivity.this).inflate(R.layout.layout_item_rectangle, parent, false);
      return new RvProductsAdapter.RvProductHolder(view);
    }

    @Override
    public void onBindViewHolder(@NonNull RvProductHolder holder, int position) {
      holder.txt_productName.setText(data.get(position).getProductName());
      holder.txt_productDescription.setText(data.get(position).getDescription());
      holder.txt_price.setText(data.get(position).getPriceString());
      holder.rec_txt_item_category.setText(data.get(position).getCategory());
      holder.rec_txt_item_stock.setText(String.valueOf(data.get(position).getStockLevel()));

      holder.rec_item.setOnClickListener(v -> {
        Intent intent = new Intent(MainActivity.this, ProductActivity.class);
        intent.putExtra("productId", data.get(position).getId());
        startActivity(intent);
      });

      final String imageUrl = data.get(position).getImageUrl();
      if (imageUrl != null) {
        Glide.with(MainActivity.this).load(imageUrl).into(holder.img_productImage);
      } else {
        holder.img_productImage.setImageResource(R.drawable.product_placeholder);
      }
    }

    @Override
    public int getItemCount() {
      return data.size();
    }

    class RvProductHolder extends RecyclerView.ViewHolder {
      private final TextView txt_productName;
      private final TextView txt_productDescription;
      private final ImageView img_productImage;
      private final TextView txt_price;
      private final TextView rec_txt_item_category;
      private final TextView rec_txt_item_stock;
      private final CardView rec_item;


      public RvProductHolder(@NonNull View itemView) {
        super(itemView);

        txt_productName = itemView.findViewById(R.id.rec_txt_item_label);
        txt_productDescription = itemView.findViewById(R.id.rec_txt_item_description);
        img_productImage = itemView.findViewById(R.id.rec_img_item);
        txt_price = itemView.findViewById(R.id.rec_txt_item_price);
        rec_txt_item_category = itemView.findViewById(R.id.rec_txt_item_category);
        rec_txt_item_stock = itemView.findViewById(R.id.rec_txt_item_stock);
        rec_item = itemView.findViewById(R.id.rec_item);
      }
    }


  }

}