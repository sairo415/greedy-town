package com.greedytown.domain.item.model;

import com.greedytown.domain.user.model.User;
import lombok.*;

import javax.persistence.*;
import java.util.Date;

@Entity
@Setter
@Getter
@Builder
@AllArgsConstructor
@NoArgsConstructor
public class MoneyLog {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long moneyLogSeq;

    @Column(columnDefinition = "DATETIME")
    private Date moneyLogTime;

    @Column(columnDefinition = "TINYINT")
    private Integer moneyLogGameinfo;

    private Long moneyLogMoney;

    @JoinColumn(name="money_log_iteminfo")
    @ManyToOne
    private Item moneyLogIteminfo;

    @JoinColumn(name = "user_seq")
    @ManyToOne
    private User userSeq;

}
