package com.sliit.shopease.models;

import com.google.gson.Gson;

public class Category {
  private final String name;
  private final String imageUrl;

  public Category(String name, String imageUrl) {
    this.name = name;
    this.imageUrl = imageUrl;
  }

  //GSON from Json
  public static Category fromJson(String jsonString) {
    Gson gson = new Gson();
    return gson.fromJson(jsonString, Category.class);
  }

  public static Category fromString(String categoryString) {
   return new Category(categoryString, null);
  }

  public String getName() {
    return name;
  }

  public String getImageUrl() {
    return imageUrl;
  }

  //GSON to json
  public String toJson() {
    Gson gson = new Gson();
    return gson.toJson(this);
  }
}
