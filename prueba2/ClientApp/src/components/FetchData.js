import React, { Component } from 'react';
import './styles.css';

export class FetchData extends Component {
  

  constructor(props) {
    super(props);
    this.state = {
      rows: 10,
      cols: 10,
      startCell: null,
      endCell: null,
      obstacles: [],
      caminosRealizados: 0,
      caminosBuenos: 0,
      caminosOptimos: 0,
      pesoCaminoOptimo: 0,
      path: [],
      cells: []
    };
  }

  

  getCells() {
    const cells = [];
    for (let i = 0; i < this.state.rows; i++) {
      for (let j = 0; j < this.state.cols; j++) {
        cells.push({ row: i, col: j });
      }
    }
    // this.setState({cells: cells})
    this.state.cells = cells
    
    return cells;
  }

  getRutas = async () => {
    const { startCell, endCell, obstacles } = this.state;
    
    const dt = await this.postData('weatherforecast', {
      dimensions: `${this.state.rows},${this.state.cols}`, //'10,10',
      start: `${startCell.row},${startCell.col}`,
      end: `${endCell.row},${endCell.col}`,
      obstacles: obstacles.map((obstacle) => `${obstacle.row},${obstacle.col}`).join(';'),
    });

    const data = await dt.json();
    if (data.board) {
      this.state.path = []
      data.board.forEach((item, index_i) => {
        item.forEach((item_i, index_j) => {
          if (item_i == 4) {
            this.state.path.push({row: index_i, col: index_j})
          }
        })
      });
    }

    this.state.caminosRealizados = data.caminosRealizados;
    this.state.caminosBuenos = data.caminosBuenos;
    this.state.caminosOptimos = data.caminosOptimos;
    this.state.pesoCaminoOptimo = data.pesoCaminoOptimo;
    
    const cells = this.state.cells;
    
    this.setState({cells})
    
  };

  postData = async (url = '', data = {}) => {
    
    return await fetch(url, {
      method: 'POST',
      // mode: 'no-cors',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(data),
    });
  };

  onCellClick = (cell) => {
    const { startCell, endCell, obstacles } = this.state;

    if (this.isStart(cell)) {
      this.setState({ startCell: null });
    } else if (this.isEnd(cell)) {
      this.setState({ endCell: null });
    } else if (this.isObstacle(cell)) {
      const newObstacles = obstacles.filter((obstacle) => {
        return obstacle.row !== cell.row || obstacle.col !== cell.col;
      });
      this.setState({ obstacles: newObstacles });
    } else {
      if (!startCell) {
        this.setState({ startCell: cell });
      } else if (!endCell) {
        this.setState({ endCell: cell });
      } else {
        this.setState({ obstacles: [...obstacles, cell] });
      }
    }
  };

  isStart = (cell) => {
    const { startCell } = this.state;

    return startCell && cell.row === startCell.row && cell.col === startCell.col;
  };

  isPath = (cell) => {
    const { path } = this.state;

    return path.some((path) => {
      return path.row === cell.row && path.col === cell.col;
    });
  }

  isEnd = (cell) => {
    const { endCell } = this.state;

    return endCell && cell.row === endCell.row && cell.col === endCell.col;
  };

  isObstacle = (cell) => {
    const { obstacles } = this.state;

    return obstacles.some((obstacle) => {
      return obstacle.row === cell.row && obstacle.col === cell.col;
    });
  };

  getClassName = (cell) => {
    const { start, end, obstacle, path } = {
      start: this.isStart(cell),
      end: this.isEnd(cell),
      obstacle: this.isObstacle(cell),
      path: this.isPath(cell)
    };
    let className = "cell ";
    if (start) {
      className += "start ";
    }
    if (end) {
      className += "end ";
    }
    if (obstacle) {
      className += "obstacle ";
    }
    if (path) {
      className += "path";
    }
    return className.trim();
  };
  

  
  

  render() {
    const { cols } = this.state;
    this.getCells();
    

    return (
      <div>
        <label>
          Rows:
          <input
            type="number"
            value={this.state.rows}
            onChange={(event) =>
              this.setState({ rows: parseInt(event.target.value) })
            }
            min="1"
            max="15"
          />
        </label>
        <label>
          Columns:
          <input
            type="number"
            value={this.state.cols}
            onChange={(event) =>
              this.setState({ cols: parseInt(event.target.value) })
            }
            min="1"
            max="15"
          />
        </label>
        <br />
        <div
          className="grid"
          style={{ gridTemplateColumns: `repeat(${this.state.cols}, 40px)` }}
        >
          {this.state.cells.map((cell, index) => (
            <button
              className={`cell_n ${this.getClassName(cell)}`}
              key={index}
              onClick={() => this.onCellClick(cell)}
            ></button>
          ))}
        </div>

        <button
          className="accept"
          onClick={() =>
            this.getRutas(this.state.startCell, this.state.endCell, this.state.obstacles)
          }
        >
          Obtener rutas
        </button>
          <br />
        <label>
          Cantidad caminos realizados:
          <span>{this.state.caminosRealizados}</span>
        </label>
        <br />
        <label>
          Cantidad caminos buenos:
          <span>{this.state.caminosBuenos}</span>
        </label>
        <br />
        <label>
          Cantidad caminos óptimos:
          <span>{this.state.caminosOptimos}</span>
        </label>
        <br />
        <label>
          Peso camino óptimo:
          <span>{this.state.pesoCaminoOptimo}</span>
        </label>
      </div>
    );
  }
}

