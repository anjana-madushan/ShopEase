package com.sliit.shopease.models;

public class DemoCategory {
  private String id;
  private String name;
  private String imageUrl;

  public DemoCategory(String id, String name, String imageUrl){
    this.id = id;
    this.name = name;
    this.imageUrl = imageUrl;
  }

  public String getId() {
    return id;
  }

  public String getName() {
    return name;
  }

  public String getImageUrl() {
    return imageUrl;
  }
}
