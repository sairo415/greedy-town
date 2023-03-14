package com.greedytown.domain.item.model;

import lombok.Getter;
import lombok.Setter;

import javax.persistence.*;

@Entity
@Setter
@Getter
public class ItemColor {

    @Id
    @Column(columnDefinition = "SMALLINT")
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Integer itemColorSeq;

    private String itemColorName;




}
