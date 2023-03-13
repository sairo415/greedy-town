package com.greedytown.domain.item.model;

import lombok.Getter;
import lombok.Setter;

import javax.persistence.Entity;
import javax.persistence.GeneratedValue;
import javax.persistence.GenerationType;
import javax.persistence.Id;

@Entity
@Setter
@Getter
public class ItemColor {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Short itemColorSeq;

    private String itemColorName;




}
