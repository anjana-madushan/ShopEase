package com.sliit.shopease.repository;

import android.content.Context;

import com.sliit.shopease.constants.ApiEndPoints;
import com.sliit.shopease.helpers.NetworkHelper;
import com.sliit.shopease.interfaces.NetworkCallback;
import com.sliit.shopease.models.Product;
import com.sliit.shopease.models.ShopEaseError;

import org.json.JSONArray;
import org.json.JSONObject;

import java.util.ArrayList;

public class ProductsRepository {
  private NetworkHelper networkHelper = NetworkHelper.getInstance();

  public void getAllProducts(Context context, NetworkCallback<ArrayList<Product>> callback) {
    networkHelper.get(context, ApiEndPoints.ALL_PRODUCTS, true, new NetworkCallback<String>() {
      @Override
      public void onSuccess(String response) {
        System.out.println(response);

        try {
          JSONArray items = new JSONArray(response);
          ArrayList<Product> products = new ArrayList<>();

          for (int i = 0; i < items.length(); i++) {
            JSONObject item = items.getJSONObject(i);
            Product product = Product.fromJson(item.toString());
            products.add(product);
          }

          callback.onSuccess(products);
        } catch (Exception e) {
          e.printStackTrace();
          callback.onFailure(new ShopEaseError(e));
        }
      }

      @Override
      public void onFailure(ShopEaseError error) {

      }
    });
  }

}
