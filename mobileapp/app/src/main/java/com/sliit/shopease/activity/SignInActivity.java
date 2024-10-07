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

import com.google.android.material.floatingactionbutton.FloatingActionButton;
import com.sliit.shopease.R;
import com.sliit.shopease.helpers.DialogHelper;
import com.sliit.shopease.helpers.NetworkHelper;
import com.sliit.shopease.helpers.SharedPreferencesHelper;
import com.sliit.shopease.interfaces.NetworkCallback;
import com.sliit.shopease.models.ShopEaseError;

import java.util.HashMap;
import java.util.Map;

public class SignInActivity extends AppCompatActivity {
  private EditText edt_email;
  private EditText edt_pass;
  private SharedPreferencesHelper sharedPreferencesHelper;
  private NetworkHelper networkHelper;
  private TextView txt_register;

  @Override
  protected void onCreate(Bundle savedInstanceState) {
    super.onCreate(savedInstanceState);
    EdgeToEdge.enable(this);
    setContentView(R.layout.activity_sign_in);
    ViewCompat.setOnApplyWindowInsetsListener(findViewById(R.id.activity_signIn), (v, insets) -> {
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

    edt_email = findViewById(R.id.signIn_edt_email);
    edt_pass = findViewById(R.id.signIn_edt_password);
    txt_register = findViewById(R.id.txt_register);

    Button btn_signIn = findViewById(R.id.signIn_btn_signIn);
    FloatingActionButton fab = findViewById(R.id.fab);

    sharedPreferencesHelper = new SharedPreferencesHelper(this);
    networkHelper = NetworkHelper.getInstance();

    fab.setOnClickListener(view -> showBaseUrlDialog());
    btn_signIn.setOnClickListener(this::signIn);
    txt_register.setOnClickListener(view -> goRegister());
  }

  void showBaseUrlDialog() {
    String baseUrl = sharedPreferencesHelper.getString("base_url", "");

    DialogHelper.showEdtTextDialog(this, "Base URL", baseUrl, new DialogHelper.DialogCallback() {
      @Override
      public void onOk(String inputText) {
        sharedPreferencesHelper.saveString("base_url", inputText);
      }

      @Override
      public void onCancel() {
      }
    });
  }

  void signIn(View v) {
    edt_email.clearFocus();
    edt_pass.clearFocus();

    InputMethodManager imm = (InputMethodManager) getSystemService(Context.INPUT_METHOD_SERVICE);
    if (imm != null) {
      imm.hideSoftInputFromWindow(v.getWindowToken(), 0);
    }

    DialogHelper.showLoading(this, "Signing in...");

    String email = edt_email.getText().toString();
    String password = edt_pass.getText().toString();

    if (email.isEmpty() || password.isEmpty()) {
      DialogHelper.showAlert(this, "Error", "Please enter both email and password");
      return;
    }

    final Map<String, String> jsonBody = new HashMap<>();
    jsonBody.put("email", email);
    jsonBody.put("password", password);
    jsonBody.put("role", "customer");


    networkHelper.post(this, "/api/user/login", jsonBody, new NetworkCallback() {
      @Override
      public void onSuccess(String response) {
        System.out.println(response);
        DialogHelper.hideLoading();
        DialogHelper.showAlert(SignInActivity.this, "Success", "Login successful");
      }

      @Override
      public void onFailure(ShopEaseError error) {
        runOnUiThread(() -> {
          System.out.println(error);
          DialogHelper.hideLoading();
          DialogHelper.showAlert(SignInActivity.this, "Error: ", error.getMessage());
        });
      }
    });
  }

  void goRegister(){
    Intent intent = new Intent(this, RegisterActivity.class);
    startActivity(intent);
  }
}

