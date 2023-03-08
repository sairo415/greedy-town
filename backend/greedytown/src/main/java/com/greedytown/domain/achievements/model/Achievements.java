package com.greedytown.domain.achievements.model;

import javax.persistence.*;

@Entity
public class Achievements {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long achievementsIndex;

    private String achievementsContent;




}
