package com.sliit.shopease.models;

import java.util.Locale;

public class Product {
  private String id;
  private String name;
  private String description;
  private String imageUrl;
  private double price;

  public Product(String id, String name, String description, String imageUrl, double price) {
    this.id = id;
    this.name = name;
    this.description = description;
    this.imageUrl = imageUrl;
    this.price = price;
  }

  public String getId() {
    return id;
  }

  public String getName() {
    return name;
  }

  public String getDescription() {
    return description;
  }

  public String getImageUrl() {
    return imageUrl;
  }

  public String getPriceString() {
    return String.format(Locale.ENGLISH , "Rs. %.2f", price);
  }
}
