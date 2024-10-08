import React, { useState, useEffect } from "react";
import "../styles/login.css";
import "bootstrap/dist/css/bootstrap.min.css";
import { useDispatch, useSelector } from "react-redux";
import { useNavigate } from "react-router-dom";
import loginImage from "../assets/images/login-background.jpg";
import { loginAction } from "../redux/auth/authAction";

const Login = () => {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [role, setRole] = useState("");
  const dispatch = useDispatch();
  const navigate = useNavigate();
  // const { isLoggedIn, loading, error } = useSelector((state) => state.user);

  // // useEffect to handle navigation when login is successful
  // useEffect(() => {
  //   if (isLoggedIn) {
  //     navigate("/home"); // Navigate to the home page if the user is logged in
  //   }
  // }, [isLoggedIn, navigate]); // Trigger navigation when isLoggedIn state changes

  const handleSubmit = async (e) => {
    try {
      e.preventDefault();
      await dispatch(loginAction(email, password, role, navigate));
    } catch (error) {
      alert("Error logging in");
      console.error("Error logging in:", error);
    }
  };

  return (
    <div className="container-fluid">
      <div className="login-box d-flex">
        <div className="login-form">
          <p className="text-primary fw-bold">Shop Easy</p>
          <h1>
            Holla, <br /> Welcome Back
          </h1>
          <p>Hey, welcome back to your special place</p>
          <form onSubmit={handleSubmit}>
            <div className="mb-3">
              <input
                type="email"
                className="form-control"
                placeholder="email"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                required
              />
            </div>
            <div className="mb-3">
              <input
                type="password"
                className="form-control"
                placeholder="password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                required
              />
            </div>
            <div className="mb-3">
              <select
                className="form-select"
                value={role}
                onChange={(e) => setRole(e.target.value)}
                required
              >
                <option value="" disabled>
                  Select your role
                </option>
                <option value="vendor">Vendor</option>
                <option value="admin">Admin</option>
                <option value="csr">CSR</option>
              </select>
            </div>
            <div className="d-flex justify-content-between align-items-center">
              <div className="form-check">
                <input
                  type="checkbox"
                  className="form-check-input"
                  id="rememberMe"
                />
                <label className="form-check-label" htmlFor="rememberMe">
                  Remember me
                </label>
              </div>
              <a href="#" className="text-muted">
                Forgot Password?
              </a>
            </div>
            <button
              type="submit"
              className="btn btn-primary btn-block mt-4"
              // disabled={loading}
            >
              Sign In
            </button>
            {/* {error && <p className="text-danger mt-3">{error}</p>} */}
          </form>
          <div className="mt-3">
            <span>Don't have an account? </span>
            <a href="/register" className="text-primary">
              Sign Up
            </a>
          </div>
        </div>
        <div className="login-image-container">
          <img
            src={loginImage}
            alt="Login Illustration"
            className="login-image"
          />
        </div>
      </div>
    </div>
  );
};

export default Login;
