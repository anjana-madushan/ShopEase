package com.sliit.shopease.activity;

import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.TextView;

import androidx.activity.EdgeToEdge;
import androidx.annotation.NonNull;
import androidx.appcompat.app.AppCompatActivity;
import androidx.core.graphics.Insets;
import androidx.core.view.ViewCompat;
import androidx.core.view.WindowInsetsCompat;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import com.sliit.shopease.R;
import com.sliit.shopease.models.Cart;
import com.sliit.shopease.models.Product;

import java.util.Map;

public class ShoppingCartActivity extends AppCompatActivity {

  @Override
  protected void onCreate(Bundle savedInstanceState) {
    super.onCreate(savedInstanceState);
    EdgeToEdge.enable(this);
    setContentView(R.layout.activity_shopping_cart);
    ViewCompat.setOnApplyWindowInsetsListener(findViewById(R.id.main), (v, insets) -> {
      Insets systemBars = insets.getInsets(WindowInsetsCompat.Type.systemBars());
      v.setPadding(systemBars.left, systemBars.top, systemBars.right, systemBars.bottom);
      return insets;
    });

    Cart cart = new Cart(this);
    RecyclerView rv_cart = findViewById(R.id.cart_rv_items);

    LinearLayoutManager linearLayoutManager = new LinearLayoutManager(ShoppingCartActivity.this, LinearLayoutManager.VERTICAL, false);
    RvAdapter rvAdapter = new RvAdapter(cart);
    rv_cart.setLayoutManager(linearLayoutManager);
    rv_cart.setAdapter(rvAdapter);
  }

  public class RvAdapter extends RecyclerView.Adapter<RvAdapter.RvHolder> {
    private final Map<String, Integer> data;

    public RvAdapter(Cart cart) {
      this.data = cart.getItems();
    }

    @NonNull
    @Override
    public RvHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
      View view = LayoutInflater.from(ShoppingCartActivity.this).inflate(R.layout.layout_cart_item, parent, false);
      return new RvHolder(view);
    }

    @Override
    public void onBindViewHolder(@NonNull RvHolder holder, int position) {
      final String productJson = data.keySet().toArray()[position].toString();
      final Product product = Product.fromJson(productJson);

      holder.cartItem_txt_name.setText(product.getProductName());
      holder.cardItem_txt_price.setText(product.getPriceString());
      holder.cardItem_txt_count.setText(String.valueOf(data.get(productJson)));
    }

    @Override
    public int getItemCount() {
      return data.size();
    }


    private class RvHolder extends RecyclerView.ViewHolder {
      private final TextView cartItem_txt_name;
      private final TextView cardItem_txt_price;
      private final TextView cardItem_txt_count;

      public RvHolder(@NonNull View itemView) {
        super(itemView);

        cartItem_txt_name = itemView.findViewById(R.id.cartItem_txt_name);
        cardItem_txt_price = itemView.findViewById(R.id.cardItem_txt_price);
        cardItem_txt_count = itemView.findViewById(R.id.cardItem_txt_count);
      }
    }
  }

}