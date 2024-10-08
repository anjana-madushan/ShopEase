/* eslint-disable no-unused-vars */
import { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import ProductModel from "./productModal";
import { getAllProducts, createProduct, deleteProductByID,getAllProductsAdmin } from "../../api/services/productService";
import ProductViewModal from "./ViewProduct";
import ViewProductModal from "./ViewProductModel";
import '../../styles/table.css'; 
export default function Products() {
  const [productData, setProductData] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [success, setSuccess] = useState(null);
  const [showModal, setShowModal] = useState(false);
  const [showViewModal, setShowViewModal] = useState(false);
  const [selectedProductId, setSelectedProductId] = useState(null);

  // Get the token from the Redux store
  const token = useSelector((state) => state.auth.loggedUser.token);
  const role = useSelector((state) => state.auth.loggedUser.role);
  console.log("Role:", role); // Log the role value

  useEffect(() => {
    try {
      if(role === "vender"){
        if (token) {
          getAllProducts(token)
            .then((response) => {
              console.log("API Response:", response.data); // Log the API response
              setProductData(response); // Store the array of admin objects in state
              setLoading(false);
              console.log("vendor Data:", productData);
            })
            .catch((error) => {
              console.error(
                "Error fetching vendor profiles:",
                error.response || error
              ); // Log the error
              setError("Failed to fetch vendor profiles");
              setLoading(false);
            });
        } else {
          console.error("No token found. Please log in.");
          setError("No token found. Please log in.");
          setLoading(false);
        }
      }
      else if(role === "admin" || role === "csr"){
        if (token) {
          getAllProductsAdmin(token)
            .then((response) => {
              console.log("API Response:", response.data); // Log the API response
              setProductData(response); // Store the array of admin objects in state
              setLoading(false);
              console.log("vendor Data:", productData);
              
            })
            .catch((error) => {
              console.error(
                "Error fetching vendor profiles:",
                error.response || error
              ); // Log the error
              setError("Failed to fetch vendor profiles");
              setLoading(false);
            });
        } else {
          console.error("No token found. Please log in.");
          setError("No token found. Please log in.");
          setLoading(false);
        }
      }
    } catch (error) {
      alert("Error fetching admin profiles");
      console.error("Error fetching admin profiles:", error);
      setError("Failed to fetch admin profiles");
      setLoading(false);
    }
  }, [token]);


  const handleAddProducts = async (newProduct) => {
    if (!token) {
      setError("No token found. Please log in.");
      return;
    }
    try {
      setError(null);
      setSuccess(null);
      const response = await createProduct(newProduct.productName, newProduct.price, newProduct.category, newProduct.description, newProduct.isActive, newProduct.stockLevel, newProduct.minStockLevel, token);

      if (response.newProduct) {
        setSuccess("Product added successfully!");
        setProductData((prevData) => [...prevData, response.newProduct]);
      } else {
        console.error("Failed to add Product: No new admin in response");
        setError("Failed to add Product: Unexpected API response");
      }
    } catch (error) {
      console.error("Error adding Product:", error);
      setError("Error adding Product: " + (error.response?.data?.message || error.message));
    }
  };

  const handleDelete = async (productId) => {
    if (!token) {
      setError("No token found. Please log in.");
      return;
    }
    // Confirmation dialog
    const confirmDelete = window.confirm("Are you sure you want to delete this product?");

    if (!confirmDelete) {
      return;
    }

    try {
      setError(null);
      setSuccess(null);
      const response = await deleteProductByID(productId, token);

      if (response.status === 200) {
        setSuccess("Product deleted successfully!");
        setProductData((prevData) => prevData.filter((product) => product.id !== productId));
      } else {
        console.error("Failed to delete Product: Unexpected API response");
        setError("Failed to delete Product: Unexpected API response");
      }
    } catch (error) {
      console.error("Error deleting Product:", error);
      setError("Error deleting Product: " + (error.response?.data?.message || error.message));
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

  const handleViewProduct = (productId) => {
    setSelectedProductId(productId);
    setShowViewModal(true);
  };
  const handleCloseViewModal = () => setShowViewModal(false);

  // Render the table with admin profiles
  return (

    <div className="container">
      <div style={{ paddingBottom: "20px" }}>
        <button
          onClick={handleShowModal}
          className="btn btn-primary btn-block mt-4"
          style={{ padding: "10px 20px", width: "100px", fontSize: "16px", fontWeight: "bold" }}
        >
          Add
        </button>
      </div>

      <div className="table-wrapper" >
        <table className="table custom-table">
          <thead>
            <tr>
              <th scope="col">Name</th>
              <th scope="col">Price</th>
              <th scope="col">Category</th>
              <th scope="col">Category Status</th>
              <th scope="col">Product Status</th>
              <th scope="col">Actions</th>
            </tr>
          </thead>
          <tbody>
            {productData?.length > 0 ? (
              productData.map((product) => (
                <tr key={product.id}>
                  <td>{product.productName}</td>
                  <td>{product.price}</td>
                  <td>{product.category}</td>
                  <td>{product.isCategoryActive ? "Active" : "Deactive"}</td>
                  <td>{product.isActive ? "Active" : "Deactive"}</td>
                  <td><button className="btn btn-primary btn-sm" onClick={() => handleViewProduct(product.id)}>View</button>
                  {role === "vendor" ? <button className="btn btn-danger btn-sm" onClick={() => handleDelete(product.id)}>Delete</button>
                  :null}</td>
                    
                </tr>
              ))
            ) : (
              <tr>
                <td colSpan="3">Prodcuts not found</td>
              </tr>
            )}
          </tbody>
        </table>
      </div>
      <ProductModel
        show={showModal}
        handleClose={handleCloseModal}
        handleAddUser={handleAddProducts}
      />
      {role === "vendor" ?< ProductViewModal
        show={showViewModal}
        handleClose={handleCloseViewModal}
        productId={selectedProductId}
      /> : <ViewProductModal
        show={showViewModal}
        handleClose={handleCloseViewModal}
        productId={selectedProductId}/>

      }
      
    </div>
  );
}
