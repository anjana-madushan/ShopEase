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
import androidx.core.graphics.Insets;
import androidx.core.view.ViewCompat;
import androidx.core.view.WindowInsetsCompat;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import com.bumptech.glide.Glide;
import com.sliit.shopease.R;
import com.sliit.shopease.constants.PrefKeys;
import com.sliit.shopease.helpers.SharedPreferencesHelper;
import com.sliit.shopease.models.Category;
import com.sliit.shopease.models.Product;

import java.util.ArrayList;
import java.util.Collections;

public class MainActivity extends AppCompatActivity {
  RecyclerView rv_categories;
  RecyclerView rv_products;
  ImageButton btn_profile;

  ArrayList<Category> categoryData;
  ArrayList<Product> productData;
  LinearLayoutManager h_linearLayoutManager;
  LinearLayoutManager v_linearLayoutManager;
  RvCategoriesAdapter rvCategoriesAdapter;
  RvProductsAdapter rvProductsAdapter;

  @Override
  protected void onCreate(Bundle savedInstanceState) {
    checkUser();

    super.onCreate(savedInstanceState);
    EdgeToEdge.enable(this);
    setContentView(R.layout.activity_main);
    ViewCompat.setOnApplyWindowInsetsListener(findViewById(R.id.main), (v, insets) -> {
      Insets systemBars = insets.getInsets(WindowInsetsCompat.Type.systemBars());
      v.setPadding(systemBars.left, systemBars.top, systemBars.right, systemBars.bottom);
      return insets;
    });

    rv_categories = findViewById(R.id.rv_categories);
    rv_products = findViewById(R.id.rv_items);
    btn_profile = findViewById(R.id.btn_profile);

    btn_profile.setOnClickListener(v -> goToProfile());

    categoryData = new ArrayList<>();
    categoryData.add(new Category("1", "Category 1", "https://picsum.photos/200/300"));
    categoryData.add(new Category("2", "Category 2", "https://picsum.photos/200/100"));
    categoryData.add(new Category("3", "Category 3", "https://picsum.photos/100/300"));
    categoryData.add(new Category("4", "Category 4", "https://picsum.photos/220/300"));
    categoryData.add(new Category("5", "Category 5", "https://picsum.photos/230/300"));
    categoryData.add(new Category("6", "Category 6", "https://picsum.photos/240/300"));
    categoryData.add(new Category("7", "Category 7", "https://picsum.photos/250/300"));

    productData = new ArrayList<>();
    productData.add(new Product("1", "Product 1", "random description", "https://picsum.photos/300/300", 1000.00));
    productData.add(new Product("2", "Product 2", "random description", "https://picsum.photos/400/300", 200));
    productData.add(new Product("3", "Product 3", "random description", "https://picsum.photos/500/300", 150));
    productData.add(new Product("4", "Product 4", "random description", "https://picsum.photos/600/300", 1000));
    productData.add(new Product("5", "Product 5", "random description", "https://picsum.photos/700/300", 10));
    productData.add(new Product("6", "Product 6", "random description", "https://picsum.photos/800/300", 500));
    productData.add(new Product("7", "Product 7", "random description", "https://picsum.photos/900/300", 560));
    productData.add(new Product("8", "Product 8", "random description", "https://picsum.photos/000/300", 9000));
    productData.add(new Product("9", "Product 9", "random description", "https://picsum.photos/250/300", 50));
    productData.add(new Product("0", "Product 10", "random description", "https://picsum.photos/250/300", 300));

    h_linearLayoutManager = new LinearLayoutManager(MainActivity.this, LinearLayoutManager.HORIZONTAL, false);
    rvCategoriesAdapter = new RvCategoriesAdapter(categoryData);
    rv_categories.setLayoutManager(h_linearLayoutManager);
    rv_categories.setAdapter(rvCategoriesAdapter);

    v_linearLayoutManager = new LinearLayoutManager(MainActivity.this, LinearLayoutManager.VERTICAL, false);
    rvProductsAdapter = new RvProductsAdapter(productData);
    rv_products.setLayoutManager(v_linearLayoutManager);
    rv_products.setAdapter(rvProductsAdapter);
  }

  private void goToProfile() {
    Intent intent = new Intent(MainActivity.this, ProfileActivity.class);
    startActivity(intent);
  }

  private void checkUser() {
    SharedPreferencesHelper sharedPreferencesHelper = new SharedPreferencesHelper(MainActivity.this);
    String token = sharedPreferencesHelper.getString(PrefKeys.USER, "");

    if (token.isEmpty()) {
      Intent intent = new Intent(MainActivity.this, SignInActivity.class);
      startActivity(intent);
      finish();
    }
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
      Glide.with(MainActivity.this).load(data.get(position).getImageUrl()).into(holder.img_categoryImage);
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
      holder.txt_productName.setText(data.get(position).getName());
      holder.txt_productDescription.setText(data.get(position).getDescription());
      holder.txt_price.setText(data.get(position).getPriceString());

      Glide.with(MainActivity.this).load(data.get(position).getImageUrl()).into(holder.img_productImage);
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

      public RvProductHolder(@NonNull View itemView) {
        super(itemView);

        txt_productName = itemView.findViewById(R.id.rec_txt_item_label);
        txt_productDescription = itemView.findViewById(R.id.rec_txt_item_description);
        img_productImage = itemView.findViewById(R.id.rec_img_item);
        txt_price = itemView.findViewById(R.id.rec_txt_item_price);
      }
    }


  }

}