import React, { useState, useEffect, useRef } from "react";
import AddShoppingCartIcon from "@material-ui/icons/AddShoppingCart";
import {
  IconButton,
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
} from "@material-ui/core";
import PropTypes from "prop-types";
import toastr from "toastr";
import * as cartService from "../../services/cartService";
import logger from "sabio-debug";

const _logger = logger.extend("AddToCart");

const AddToCart = (props) => {
  _logger(props);
  const [open, setOpen] = useState(false); // to use removal window

  toastr.options = {
    onclick: null,
    fadeIn: 300,
    fadeOut: 1000,
    timeOut: 3000,
    extendedTimeOut: 1000,
    positionClass: "toast-top-center",
  };

  const handleClick = () => {
    _logger(
      "ProvService Id and Provider Id:",
      props.providerServiceId,
      props.providerId
    );

    let providerServiceId = {
      providerServiceId: props.providerServiceId,
    };

    const providerId = props.providerId;

    // if current provId !== incoming provId, ask user if they want to start new cart
    // if clicked yes, emptyCart ajax -> remove all items from localStorage
    // for later implementation...

    cartService
      .addItemToCart(providerServiceId)
      .then(itemAddSuccess)
      .catch(itemAddError);
  };

  const itemAddSuccess = (response) => {
    _logger(response);
    if (response.isSuccessful && response.item === 0) {
      toastr.clear();
      toastr.info("Item Already in Cart!");
    } else {
      toastr.clear();
      toastr.success("Item Added to Cart!");

      let oldItems = JSON.parse(localStorage.getItem("itemsArray")) || [];
      let itemId = response.item;
      oldItems.splice(oldItems.length - 1, 0, itemId);
      localStorage.setItem("itemsArray", JSON.stringify(oldItems));
      props.manageCartCount(oldItems.length);
    }
  };

  const itemAddError = (err) => {
    _logger(err);
    toastr.error("Failed To Add");
  };

  const handleOpen = () => {
    setOpen(true);
  };

  const handleClose = () => {
    setOpen(false);
  };

  const resetCart = () => {
    cartService
      .emptyCartByUserId()
      .then(emptyCartByUserIdSuccess)
      .catch(emptyCartByUserIdError);
  };

  const emptyCartByUserIdSuccess = (response) => {
    _logger(response);
    localStorage.clear("itemsArray");
  };

  const emptyCartByUserIdError = (err) => {
    _logger(err);
  };

  return (
    <>
      <IconButton
        onClick={handleClick}
        color="inherit"
        className="btn-inverse mx-1 d-50"
      >
        <AddShoppingCartIcon fontSize="small">Add to Cart</AddShoppingCartIcon>
      </IconButton>
      <div>
        <Dialog
          open={open}
          onClose={handleClose}
          aria-labelledby="alert-dialog-title"
          aria-describedby="alert-dialog-description"
        >
          <DialogTitle id="alert-dialog-title">
            {
              "Your cart already consists of services from a different provider. Would you like to reset existing cart?"
            }
          </DialogTitle>
          <DialogContent></DialogContent>
          <DialogActions>
            <Button onClick={resetCart} color="primary">
              Yes
            </Button>
            <Button onClick={handleClose} color="primary" autoFocus>
              Cancel
            </Button>
          </DialogActions>
        </Dialog>
      </div>
    </>
  );
};

AddToCart.propTypes = {
  manageCartCount: PropTypes.func,
  onCartUpdate: PropTypes.func,
  providerId: PropTypes.number.isRequired,
  providerServiceId: PropTypes.number,
};

export default React.memo(AddToCart);