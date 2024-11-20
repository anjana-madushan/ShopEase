package com.sliit.shopease.models;

import androidx.annotation.NonNull;

import com.google.gson.Gson;
import com.google.gson.annotations.SerializedName;

import java.util.Locale;

public class Product {
  private final String id;
  private final String productName;
  private final double price;
  private final String category;
  private final String description;
  private final int stockLevel;
  private final String imageUrl;

  @SerializedName("venderId")
  private final String vendorId;

  public Product(
      String id,
      String vendorId,
      String productName,
      double price,
      String category,
      int stockLevel,
      String description,
      String imageUrl
  ) {
    this.id = id;
    this.vendorId = vendorId;
    this.productName = productName;
    this.price = price;
    this.category = category;
    this.stockLevel = stockLevel;
    this.description = description;
    this.imageUrl = imageUrl;
  }

  // Method to convert JSON to Product object
  public static Product fromJson(String jsonString) {
    Gson gson = new Gson();
    return gson.fromJson(jsonString, Product.class);
  }

  public String getId() {
    return id;
  }

  public String getVendorId() {
    return vendorId;
  }

  public String getProductName() {
    return productName;
  }

  public double getPrice() {
    return price;
  }

  public String getPriceString() {
    return String.format(Locale.ENGLISH , "Rs. %.2f", price);
  }

  public String getCategory() {
    return category;
  }

  public String getDescription() {
    return description;
  }

  public int getStockLevel() {
    return stockLevel;
  }

  public String getImageUrl() {
    return imageUrl;
  }

  // Method to convert Product object to JSON
  public String toJson() {
    Gson gson = new Gson();
    return gson.toJson(this);
  }

  @NonNull
  @Override
  public String toString() {
    return this.toJson();
  }
}
