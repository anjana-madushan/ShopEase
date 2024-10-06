package com.sliit.shopease.activity;

import android.content.Intent;
import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
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
import com.sliit.shopease.Config;
import com.sliit.shopease.R;
import com.sliit.shopease.models.Category;

import java.util.ArrayList;

public class MainActivity extends AppCompatActivity {
  RecyclerView rv_categories;
  ArrayList<Category> categoryData;
  LinearLayoutManager linearLayoutManager;
  RvCategoriesAdapter rvCategoriesAdapter;

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


    categoryData = new ArrayList<>();
    categoryData.add(new Category("1", "Category 1", "https://picsum.photos/200/300"));
    categoryData.add(new Category("2", "Category 2", "https://picsum.photos/200/100"));
    categoryData.add(new Category("3", "Category 3", "https://picsum.photos/100/300"));
    categoryData.add(new Category("4", "Category 4", "https://picsum.photos/220/300"));
    categoryData.add(new Category("5", "Category 5", "https://picsum.photos/230/300"));
    categoryData.add(new Category("6", "Category 6", "https://picsum.photos/240/300"));
    categoryData.add(new Category("7", "Category 7", "https://picsum.photos/250/300"));

    linearLayoutManager = new LinearLayoutManager(MainActivity.this, LinearLayoutManager.HORIZONTAL, false);
    rvCategoriesAdapter = new RvCategoriesAdapter(categoryData);
    rv_categories.setLayoutManager(linearLayoutManager);
    rv_categories.setAdapter(rvCategoriesAdapter);
  }

  private void checkUser() {
    if (!Config.isSignedIn) {
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
      Glide
          .with(MainActivity.this)
          .load(data.get(position).getImageUrl())
          .into(holder.img_categoryImage);
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

}