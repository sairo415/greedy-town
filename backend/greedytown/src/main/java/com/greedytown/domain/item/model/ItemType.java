package com.greedytown.domain.item.model;

import lombok.Getter;
import lombok.Setter;

import javax.persistence.*;

@Entity
@Setter
@Getter
public class ItemType {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    @Column(columnDefinition = "SMALLINT")
    private Integer itemTypeSeq;

    private String itemTypeName;




}
