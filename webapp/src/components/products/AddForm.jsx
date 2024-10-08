import React from 'react'
import '../../styles/form.css'; 

export default function AddForm() {
  return (
    <div >
      <div className="row justify-content-center">
        <div className="col-lg-10"> {/* Adjust col size here */}
          <div className="form-container">
            <h5 className="mb-4">Basic Information</h5>

            {/* Email */}
            <div className="row mb-3">
              <div className="col-md-6">
                <label className="form-label">Email <span className="text-danger">*</span></label>
                <input type="email" className="form-control" placeholder="contact@example.com" />
              </div>
              <div className="col-md-6">
                <label className="form-label">Full Name</label>
                <input type="text" className="form-control" placeholder="Full Name" />
              </div>
            </div>

            {/* First and Last Name */}
            <div className="row mb-3">
              <div className="col-md-6">
                <label className="form-label">First Name</label>
                <input type="text" className="form-control" placeholder="First Name" />
              </div>
              <div className="col-md-6">
                <label className="form-label">Last Name</label>
                <input type="text" className="form-control" placeholder="Last Name" />
              </div>
            </div>

            {/* Status and Tracking */}
            <div className="row mb-3">
              <div className="col-md-6">
                <label className="form-label">Status <span className="text-danger">*</span></label>
                <input type="text" className="form-control" placeholder="Designer" />
              </div>
              <div className="col-md-6">
                <label className="form-label">Tracking <span className="text-danger">*</span></label>
                <input type="text" className="form-control" placeholder="Tracking Status" />
              </div>
            </div>

            <h5 className="mb-4">Address Information</h5>

            {/* Address */}
            <div className="row mb-3">
              <div className="col-md-12">
                <label className="form-label">Address</label>
                <input type="text" className="form-control" placeholder="Address" />
              </div>
            </div>

            {/* City, State, ZIP */}
            <div className="row mb-3">
              <div className="col-md-4">
                <label className="form-label">City</label>
                <input type="text" className="form-control" placeholder="City" />
              </div>
              <div className="col-md-4">
                <label className="form-label">State</label>
                <input type="text" className="form-control" placeholder="State" />
              </div>
              <div className="col-md-4">
                <label className="form-label">ZIP</label>
                <input type="text" className="form-control" placeholder="ZIP" />
              </div>
            </div>

            <button className="btn btn-primary mt-3">Submit</button>
          </div>
        </div>
      </div>
    </div>
  );
}
