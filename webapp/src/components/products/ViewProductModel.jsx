/* eslint-disable react/prop-types */
import { useEffect, useState } from "react";
import { Modal, Button, Form } from "react-bootstrap";
import { getProductByID } from "../../api/services/productService";
import { useSelector } from "react-redux";

export default function ViewProductModal({ show, handleClose, productId }) {
  const [product, setProduct] = useState({
    productName: '',
    price: '',
    category: '',
    image: null,
    isCategoryActive: false,
    description: '',
    isActive: false,
    stockLevel: 0,
    minStockLevel: 0,
  });

  const token = useSelector((state) => state.auth.loggedUser.token);

  useEffect(() => {
    if (productId) {
      getProductByID(productId).then((response) => {
        setProduct(response);
      }).catch((error) => {
        console.error("Failed to fetch product details", error);
      });
    }
  }, [productId]);

  return (
    <Modal show={show} onHide={handleClose} size="lg">
      <Modal.Header closeButton>
        <Modal.Title>Product Details</Modal.Title>
      </Modal.Header>
      <Modal.Body>
        <Form.Group className="mb-3">
          <Form.Label>Product Name</Form.Label>
          <Form.Text>{product.productName}</Form.Text>
        </Form.Group>

        <Form.Group className="mb-3">
          <Form.Label>Price</Form.Label>
          <Form.Text>{product.price}</Form.Text>
        </Form.Group>

        <Form.Group className="mb-3">
          <Form.Label>Category</Form.Label>
          <Form.Text>{product.category}</Form.Text>
        </Form.Group>

        <Form.Group className="mb-3">
          <Form.Label>Description</Form.Label>
          <Form.Text>{product.description}</Form.Text>
        </Form.Group>

        <Form.Group className="mb-3">
          <Form.Label>Stock Level</Form.Label>
          <Form.Text>{product.stockLevel}</Form.Text>
        </Form.Group>

        <Form.Group className="mb-3">
          <Form.Label>Minimum Stock Level</Form.Label>
          <Form.Text>{product.minStockLevel}</Form.Text>
        </Form.Group>

        <Form.Group className="mb-3">
          <Form.Label>Is Category Active?</Form.Label>
          <Form.Text>{product.isCategoryActive ? 'Yes' : 'No'}</Form.Text>
        </Form.Group>

        <Form.Group className="mb-3">
          <Form.Label>Is Product Active?</Form.Label>
          <Form.Text>{product.isActive ? 'Yes' : 'No'}</Form.Text>
        </Form.Group>
      </Modal.Body>
      <Modal.Footer>
        <Button variant="secondary" onClick={handleClose}>
          Close
        </Button>
      </Modal.Footer>
    </Modal>
  );
}
