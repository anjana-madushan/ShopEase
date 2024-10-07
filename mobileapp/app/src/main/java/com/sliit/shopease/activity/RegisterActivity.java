package com.sliit.shopease.activity;

import android.os.Bundle;

import androidx.activity.EdgeToEdge;
import androidx.appcompat.app.AppCompatActivity;
import androidx.core.graphics.Insets;
import androidx.core.view.ViewCompat;
import androidx.core.view.WindowInsetsCompat;

import com.sliit.shopease.R;

public class RegisterActivity extends AppCompatActivity {

  @Override
  protected void onCreate(Bundle savedInstanceState) {
    super.onCreate(savedInstanceState);
    EdgeToEdge.enable(this);
    setContentView(R.layout.activity_register);
    ViewCompat.setOnApplyWindowInsetsListener(findViewById(R.id.activity_register), (v, insets) -> {
      Insets systemBarsInsets = insets.getInsets(WindowInsetsCompat.Type.systemBars());

      // Apply system bar insets (status and navigation bar)
      v.setPadding(systemBarsInsets.left, systemBarsInsets.top, systemBarsInsets.right, 0);

      // Handle the keyboard (IME) inset
      Insets imeInsetsType = insets.getInsets(WindowInsetsCompat.Type.ime());
      if (insets.isVisible(WindowInsetsCompat.Type.ime())) {
        // Keyboard is visible, add padding to avoid overlapping
        v.setPadding(systemBarsInsets.left, systemBarsInsets.top, systemBarsInsets.right, imeInsetsType.bottom);
      }

      return insets;
    });
  }
}