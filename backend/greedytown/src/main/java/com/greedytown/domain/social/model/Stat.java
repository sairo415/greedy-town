package com.greedytown.domain.social.model;

import com.greedytown.domain.user.model.User;
import lombok.*;

import javax.persistence.*;
import java.io.Serializable;
import java.sql.Timestamp;
import java.util.Date;
import java.util.Objects;


@Entity
@Getter
@Setter
@AllArgsConstructor
@NoArgsConstructor
public class Stat {
    @Id
    @Column(name="stat_seq")
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long statSeq;

    @MapsId
    @OneToOne
    @JoinColumn(name="user_seq")
    private User userSeq;

    @Column(columnDefinition = "TIME")
    private Timestamp userClearTime;


}
