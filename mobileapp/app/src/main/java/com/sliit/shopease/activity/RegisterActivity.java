package com.sliit.shopease.activity;

import android.content.Context;
import android.content.Intent;
import android.os.Bundle;
import android.view.View;
import android.view.inputmethod.InputMethodManager;
import android.widget.Button;
import android.widget.EditText;
import android.widget.TextView;

import androidx.activity.EdgeToEdge;
import androidx.appcompat.app.AppCompatActivity;
import androidx.core.graphics.Insets;
import androidx.core.view.ViewCompat;
import androidx.core.view.WindowInsetsCompat;

import com.sliit.shopease.R;
import com.sliit.shopease.constants.ApiEndPoints;
import com.sliit.shopease.helpers.DialogHelper;
import com.sliit.shopease.helpers.NetworkHelper;
import com.sliit.shopease.interfaces.NetworkCallback;
import com.sliit.shopease.models.ShopEaseError;

import java.util.HashMap;
import java.util.Map;

public class RegisterActivity extends AppCompatActivity {
  TextView reg_txt_login;
  EditText reg_edt_name;
  EditText reg_edt_email;
  EditText reg_edt_password;
  EditText reg_edt_con_password;
  Button reg_btn_reg;

  NetworkHelper networkHelper;

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

    reg_txt_login = findViewById(R.id.reg_txt_login);
    reg_edt_name = findViewById(R.id.reg_edt_name);
    reg_edt_email = findViewById(R.id.reg_edt_email);
    reg_edt_password = findViewById(R.id.reg_edt_password);
    reg_edt_con_password = findViewById(R.id.reg_edt_con_password);
    reg_btn_reg = findViewById(R.id.btn_reg);

    networkHelper = NetworkHelper.getInstance();

    reg_txt_login.setOnClickListener(v -> goToLogin(false));
    reg_btn_reg.setOnClickListener(this::register);
  }

  void goToLogin(boolean showSuccessMessage) {
    Intent intent = new Intent(this, SignInActivity.class);
    if (showSuccessMessage) {
      intent.putExtra("REGISTRATION_SUCCESS", true); // Pass success flag
    }

    startActivity(intent);
    finish();
  }

  void register(View v) {
    reg_edt_name.clearFocus();
    reg_edt_email.clearFocus();
    reg_edt_password.clearFocus();
    reg_edt_con_password.clearFocus();

    InputMethodManager imm = (InputMethodManager) getSystemService(Context.INPUT_METHOD_SERVICE);
    if (imm != null) {
      imm.hideSoftInputFromWindow(v.getWindowToken(), 0);
    }

    String name = reg_edt_name.getText().toString().trim();
    String email = reg_edt_email.getText().toString().trim();
    String password = reg_edt_password.getText().toString().trim();
    String conPassword = reg_edt_con_password.getText().toString().trim();

    if (name.isEmpty() || email.isEmpty() || password.isEmpty() || conPassword.isEmpty()) {
      DialogHelper.showAlert(this, "Error", "Please fill all the fields");
      return;
    }

    if (!password.equals(conPassword)) {
      DialogHelper.showAlert(this, "Error", "Password and Confirm Password does not match");
      return;
    }

    DialogHelper.showLoading(this, "Registering...");

    final Map<String, String> jsonBody = new HashMap<>();
    jsonBody.put("userName", name);
    jsonBody.put("email", email);
    jsonBody.put("password", password);

    networkHelper.post(this, ApiEndPoints.REGISTER, jsonBody, new NetworkCallback<String>() {
      @Override
      public void onSuccess(String response) {
        System.out.println(response);
        goToLogin(true);
        DialogHelper.hideLoading();
      }

      @Override
      public void onFailure(ShopEaseError error) {
        System.out.println(error);
        DialogHelper.hideLoading();

        runOnUiThread(() -> DialogHelper.showAlert(RegisterActivity.this, "Error: ", error.getMessage()));
      }
    });
  }
}