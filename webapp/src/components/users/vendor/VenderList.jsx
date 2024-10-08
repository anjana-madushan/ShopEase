import { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import { createVender, getAllUsers } from "../../../api/services/authService";
import UserModel from "../UserModel";

export default function VenderList() {
  const [vendorData, setVendorData] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [success, setSuccess] = useState(null);
  const [showModal, setShowModal] = useState(false);

  // Get the token from the Redux store
  const token = useSelector((state) => state.auth.loggedUser.token);

  useEffect(() => {
    try {
      if (token) {
        getAllUsers(token, "vendor")
          .then((response) => {
            console.log("API Response:", response.data); // Log the API response
            setVendorData(response); // Store the array of admin objects in state
            setLoading(false);
            //console.log("Admin Data:", adminData);
          })
          .catch((error) => {
            console.error(
              "Error fetching admin profiles:",
              error.response || error
            ); // Log the error
            setError("Failed to fetch admin profiles");
            setLoading(false);
          });
      } else {
        console.error("No token found. Please log in.");
        setError("No token found. Please log in.");
        setLoading(false);
      }
    } catch (error) {
      alert("Error fetching admin profiles");
      console.error("Error fetching admin profiles:", error);
      setError("Failed to fetch admin profiles");
      setLoading(false);
    }
  }, [token]);

  const handleAddVendor = async (newCsr) => {
    if (!token) {
      setError("No token found. Please log in.");
      return;
    }
    try {
      setError(null);
      setSuccess(null);
      const response = await createVender(
        newCsr.username,
        newCsr.password,
        newCsr.email,
        token
      );

      if (response.newCsr) {
        setSuccess("Admin added successfully!");
        setVendorData((prevData) => [...prevData, response.newCsr]);
      } else {
        console.error("Failed to add admin: No new admin in response");
        setError("Failed to add admin: Unexpected API response");
      }
    } catch (error) {
      console.error("Error adding admin:", error);
      setError(
        "Error adding admin: " +
          (error.response?.data?.message || error.message)
      );
    }
  };

  const handleCloseModal = () => setShowModal(false);
  const handleShowModal = () => setShowModal(true);
  // Render loading message while data is being fetched
  if (loading) {
    return <p>Loading...</p>;
  }

  // Render error message if fetching failed
  if (error) {
    return <p>{error}</p>;
  }

  // Render the table with admin profiles
  return (
    <div className="container">
      <div style={{ paddingBottom: "20px" }}>
        <button
          onClick={handleShowModal}
          className="btn btn-primary btn-block mt-4"
          style={{
            padding: "10px 20px",
            width: "100px",
            fontSize: "16px",
            fontWeight: "bold",
          }}
        >
          Add
        </button>
      </div>

      <div className="table-wrapper">
        <table className="table custom-table">
          <thead>
            <tr>
              <th scope="col">Username</th>
              <th scope="col">Email</th>
              <th scope="col">Action</th>
            </tr>
          </thead>
          <tbody>
            {vendorData?.length > 0 ? (
              vendorData.map((vendor) => (
                <tr key={vendor.id}>
                  <td>{vendor.username}</td>
                  <td>{vendor.email}</td>
                  <td>
                    <button className="btn btn-primary btn-sm">Details</button>
                  </td>
                </tr>
              ))
            ) : (
              <tr>
                <td colSpan="3">No admin profiles found</td>
              </tr>
            )}
          </tbody>
        </table>
      </div>
      <UserModel
        show={showModal}
        handleClose={handleCloseModal}
        handleAddUser={handleAddVendor}
      />
    </div>
  );
}
