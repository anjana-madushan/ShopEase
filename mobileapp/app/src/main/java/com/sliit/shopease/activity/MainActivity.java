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
import com.sliit.shopease.helpers.DialogHelper;
import com.sliit.shopease.helpers.SharedPreferencesHelper;
import com.sliit.shopease.interfaces.NetworkCallback;
import com.sliit.shopease.models.Category;
import com.sliit.shopease.models.DemoProduct;
import com.sliit.shopease.models.Product;
import com.sliit.shopease.models.ShopEaseError;
import com.sliit.shopease.repository.ProductsRepository;

import java.util.ArrayList;
import java.util.Collections;

public class MainActivity extends AppCompatActivity {
  private final ProductsRepository productsRepository = new ProductsRepository();

  RecyclerView rv_categories;
  RecyclerView rv_products;
  ImageButton btn_profile;

  ArrayList<Category> categoryData;
  ArrayList<DemoProduct> demoProductData;
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
    demoProductData = new ArrayList<>();
    initiateDemoData();

    h_linearLayoutManager = new LinearLayoutManager(MainActivity.this, LinearLayoutManager.HORIZONTAL, false);
    rvCategoriesAdapter = new RvCategoriesAdapter(categoryData);
    rv_categories.setLayoutManager(h_linearLayoutManager);
    rv_categories.setAdapter(rvCategoriesAdapter);

    v_linearLayoutManager = new LinearLayoutManager(MainActivity.this, LinearLayoutManager.VERTICAL, false);

    loadData();
  }

  private void loadData() {
    DialogHelper.showLoading(MainActivity.this, "Please wait...");
    productsRepository.getAllProducts(MainActivity.this, new NetworkCallback<ArrayList<Product>>() {
      @Override
      public void onSuccess(ArrayList<Product> response) {
        rvProductsAdapter = new RvProductsAdapter(response);
        rv_products.setLayoutManager(v_linearLayoutManager);
        rv_products.setAdapter(rvProductsAdapter);
        DialogHelper.hideLoading();
        runOnUiThread(() -> DialogHelper.showAlert(MainActivity.this, "Success", "Data loaded successfully"));
      }

      @Override
      public void onFailure(ShopEaseError error) {
        DialogHelper.hideLoading();
        runOnUiThread(() -> DialogHelper.showAlert(MainActivity.this, "Error", error.getMessage()));
      }
    });
  }

  private void initiateDemoData() {
    categoryData.add(new Category("1", "Category 1", "https://picsum.photos/200/300"));
    categoryData.add(new Category("2", "Category 2", "https://picsum.photos/200/100"));
    categoryData.add(new Category("3", "Category 3", "https://picsum.photos/100/300"));
    categoryData.add(new Category("4", "Category 4", "https://picsum.photos/220/300"));
    categoryData.add(new Category("5", "Category 5", "https://picsum.photos/230/300"));
    categoryData.add(new Category("6", "Category 6", "https://picsum.photos/240/300"));
    categoryData.add(new Category("7", "Category 7", "https://picsum.photos/250/300"));

//    demoProductData.add(new DemoProduct("1", "Product 1", "random description", "https://picsum.photos/300/300", 1000.00));
//    demoProductData.add(new DemoProduct("2", "Product 2", "random description", "https://picsum.photos/400/300", 200));
//    demoProductData.add(new DemoProduct("3", "Product 3", "random description", "https://picsum.photos/500/300", 150));
//    demoProductData.add(new DemoProduct("4", "Product 4", "random description", "https://picsum.photos/600/300", 1000));
//    demoProductData.add(new DemoProduct("5", "Product 5", "random description", "https://picsum.photos/700/300", 10));
//    demoProductData.add(new DemoProduct("6", "Product 6", "random description", "https://picsum.photos/800/300", 500));
//    demoProductData.add(new DemoProduct("7", "Product 7", "random description", "https://picsum.photos/900/300", 560));
//    demoProductData.add(new DemoProduct("8", "Product 8", "random description", "https://picsum.photos/000/300", 9000));
//    demoProductData.add(new DemoProduct("9", "Product 9", "random description", "https://picsum.photos/250/300", 50));
//    demoProductData.add(new DemoProduct("0", "Product 10", "random description", "https://picsum.photos/250/300", 300));
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
      holder.txt_productName.setText(data.get(position).getProductName());
      holder.txt_productDescription.setText(data.get(position).getDescription());
      holder.txt_price.setText(data.get(position).getPriceString());
      holder.rec_txt_item_category.setText(data.get(position).getCategory());
      holder.rec_txt_item_stock.setText(String.valueOf(data.get(position).getStockLevel()));

      final String imageUrl = data.get(position).getImageUrl();
      if(imageUrl != null) {
        Glide.with(MainActivity.this).load(imageUrl).into(holder.img_productImage);
      }else{
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


      public RvProductHolder(@NonNull View itemView) {
        super(itemView);

        txt_productName = itemView.findViewById(R.id.rec_txt_item_label);
        txt_productDescription = itemView.findViewById(R.id.rec_txt_item_description);
        img_productImage = itemView.findViewById(R.id.rec_img_item);
        txt_price = itemView.findViewById(R.id.rec_txt_item_price);
        rec_txt_item_category = itemView.findViewById(R.id.rec_txt_item_category);
        rec_txt_item_stock = itemView.findViewById(R.id.rec_txt_item_stock);
      }
    }


  }

}