package com.greedytown.domain.user.controller;

import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RestController;

@RestController
public class UserController {

    @GetMapping("")
    public ResponseEntity<?> aliveCheck() {
        return new ResponseEntity<>("Alive", HttpStatus.OK);
    }
}
