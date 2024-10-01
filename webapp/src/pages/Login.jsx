import React from 'react';
import '../styles/login.css';
import 'bootstrap/dist/css/bootstrap.min.css';
import loginImage from '../assets/images/login-background.jpg'; 


const Login = () => {
    return (
        <div className="container-fluid">
            
                <div className="login-box d-flex">
                    <div className="login-form">
                        <p className="text-primary fw-bold ">Shop Easy</p>
                        <h1>Holla, <br /> Welcome Back</h1>
                        <p>Hey, welcome back to your special place</p>
                        <form>
                            <div className="mb-3">
                                <input type="email" className="form-control" placeholder="email" />
                            </div>
                            <div className="mb-3">
                                <input type="password" className="form-control" placeholder="password" />
                            </div>
                            <div className="d-flex justify-content-between align-items-center">
                                <div className="form-check">
                                    <input type="checkbox" className="form-check-input" id="rememberMe" />
                                    <label className="form-check-label" htmlFor="rememberMe">Remember me</label>
                                </div>
                                <a href="#" className="text-muted">Forgot Password?</a>
                            </div>
                            <button type="submit" className="btn btn-primary btn-block mt-4">Sign In</button>
                        </form>
                        <div className="mt-3">
                            <span>Don't have an account? </span>
                            <a href="/register" className="text-primary">Sign Up</a>
                        </div>
                    </div>
                    <div className="login-image-container">
                        <img src={loginImage} alt="Login Illustration" className="login-image" />
                    </div>
                </div>
            
        </div>
    );
};

export default Login;
