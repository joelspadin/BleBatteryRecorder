#! /usr/bin/env python3
"""
Graph the output of the battery recorder service.
"""
import argparse
from pathlib import Path
import pandas as pd
import matplotlib.pyplot as plt


DEFAULT_PATH = Path("C:/ProgramData/BLE Battery Recorder/battery.csv")


def main():
    parser = argparse.ArgumentParser(description="Graph battery data")
    parser.add_argument(
        "file",
        type=Path,
        nargs="?",
        default=DEFAULT_PATH,
        help="Path to the battery data CSV file",
    )

    args = parser.parse_args()

    df = pd.read_csv(args.file, index_col=0, parse_dates=True)

    # Split this into separate data sets per device, then graph them together.
    # There's almost certainly a better way to do this, but I have no idea what
    # I'm doing.
    ax = None
    for name, d in df.groupby("Name"):
        d = d.rename(columns={"Battery": name})
        ax = d.plot(ax=ax, ylim=(0, 100), ylabel="Battery %")

    plt.show()


if __name__ == "__main__":
    main()
