package com.sliit.shopease.models;

import android.content.Context;

import com.google.gson.Gson;
import com.google.gson.reflect.TypeToken;
import com.sliit.shopease.constants.PrefKeys;
import com.sliit.shopease.helpers.SharedPreferencesHelper;

import java.lang.reflect.Type;
import java.util.HashMap;
import java.util.Map;

public class Cart {
  private final Map<String, Integer> items;
  private final transient SharedPreferencesHelper sharedPreferencesHelper;

  public Cart(Context context) {
    sharedPreferencesHelper = new SharedPreferencesHelper(context);
    String jsonString = sharedPreferencesHelper.getString(PrefKeys.CART, "");
    if (jsonString.isEmpty()) {
      items = new HashMap<>();
    } else {
      items = Cart.fromJson(jsonString);
    }
  }

  //from json
  public static Map<String, Integer> fromJson(String jsonString) {
    Gson gson = new Gson();
    Type type = new TypeToken<Map<String, Integer>>() {}.getType();
    return gson.fromJson(jsonString, type);


  }

  private void saveCart() {
    sharedPreferencesHelper.saveString(PrefKeys.CART, this.toJson());
  }

  public void addItem(Product product) {
    String productJson = product.toJson();
    items.put(productJson, items.getOrDefault(productJson, 0) + 1);
    saveCart();
  }

  public void reduceItem(Product product) {
    String productJson = product.toJson();
    if (items.containsKey(productJson)) {
      int count = items.get(productJson);
      if (count > 1) {
        items.put(productJson, count - 1);
      } else {
        items.remove(productJson);
      }
    }
    saveCart();
  }

  public void removeItem(Product product) {
    items.remove(product.toJson());
    saveCart();
  }

  public int getProductCount(Product product) {
    return items.getOrDefault(product.toJson(), 0);
  }

  public double getTotalPrice() {
    double total = 0;
    for (Map.Entry<String, Integer> entry : items.entrySet()) {
      Product product = Product.fromJson(entry.getKey());
      total += product.getPrice() * entry.getValue();
    }
    return total;
  }

  //to json
  public String toJson() {
    Gson gson = new Gson();
    return gson.toJson(items);
  }

  public Map<String, Integer> getItems() {
    return items;
  }
}
